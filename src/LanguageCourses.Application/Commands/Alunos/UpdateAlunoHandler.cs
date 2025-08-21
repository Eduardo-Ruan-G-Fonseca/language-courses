using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Manipulador do comando <see cref="UpdateAlunoCommand"/>.
/// Responsável por atualizar os dados de um aluno e sincronizar suas matrículas,
/// garantindo as regras de negócio (unicidade de CPF/E-mail e aluno vinculado a ≥ 1 turma).
/// </summary>
public class UpdateAlunoHandler : IRequestHandler<UpdateAlunoCommand, AlunoDetalheDto>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Construtor que injeta o contexto de banco de dados.
    /// </summary>
    /// <param name="db">Contexto do Entity Framework (Unit of Work/DbContext).</param>
    public UpdateAlunoHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a atualização do aluno e a sincronização de suas turmas.
    /// </summary>
    /// <param name="request">
    /// Comando contendo o <c>Id</c> do aluno e o DTO com os novos dados (<see cref="UpdateAlunoDto"/>).
    /// </param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// <see cref="AlunoDetalheDto"/> atualizado com os dados do aluno e suas turmas.
    /// </returns>
    /// <exception cref="KeyNotFoundException">Lançada quando o aluno não é encontrado.</exception>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando e-mail/CPF já existem para outro aluno,
    /// quando o aluno ficaria sem turmas,
    /// ou quando alguma das turmas está lotada ao tentar incluí-la.
    /// </exception>
    public async Task<AlunoDetalheDto> Handle(UpdateAlunoCommand request, CancellationToken ct)
    {
        // Carrega o aluno e suas matrículas/turmas
        var aluno = await _db.Alunos
            .Include(a => a.Matriculas).ThenInclude(m => m.Turma)
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (aluno is null)
            throw new KeyNotFoundException("Aluno não encontrado.");

        var dto = request.Aluno;

        //  Normaliza CPF (aceita com/sem máscara -> persiste apenas dígitos)
        var cpfDigits = Regex.Replace(dto.Cpf ?? "", "[^0-9]", "");

        //  Regras de unicidade (e-mail e CPF únicos entre alunos)
        if (await _db.Alunos.AnyAsync(a => a.Id != aluno.Id && a.Email == dto.Email.Trim(), ct))
            throw new InvalidOperationException("E-mail já cadastrado.");
        if (await _db.Alunos.AnyAsync(a => a.Id != aluno.Id && a.Cpf == cpfDigits, ct))
            throw new InvalidOperationException("CPF já cadastrado.");

        //  Atualiza dados básicos
        aluno.Nome  = dto.Nome.Trim();
        aluno.Email = dto.Email.Trim();
        aluno.Cpf   = cpfDigits; // grava só dígitos
        aluno.Idade = dto.Idade;

        //  Regra: o aluno deve permanecer em ≥ 1 turma
        if (dto.Turmas == null || dto.Turmas.Count == 0)
            throw new InvalidOperationException("Aluno deve permanecer em pelo menos 1 turma.");

        //  Normaliza referências de turmas desejadas
        var refs = dto.Turmas.Select(t => new { Idioma = t.Idioma.Trim(), t.Numero }).ToList();
        var idiomas = refs.Select(r => r.Idioma).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var numeros = refs.Select(r => r.Numero).Distinct().ToList();

        //  Carrega as turmas correspondentes e suas matrículas (para checar capacidade)
        var turmas = await _db.Turmas
            .Where(t => numeros.Contains(t.Numero) && idiomas.Contains(t.Idioma))
            .Include(t => t.Matriculas)
            .ToListAsync(ct);

        //  Valida existência e capacidade de cada referência solicitada
        foreach (var r in refs)
        {
            var turma = turmas.FirstOrDefault(t =>
                t.Numero == r.Numero &&
                string.Equals(t.Idioma, r.Idioma, StringComparison.OrdinalIgnoreCase));

            if (turma is null)
                throw new KeyNotFoundException($"Turma {r.Idioma} {r.Numero} não encontrada.");

            // Se for uma inclusão nova (aluno ainda não está nela), validar lotação (máx. 5)
            var jaMatriculado = aluno.Matriculas.Any(m => m.TurmaId == turma.Id);
            var ocupacao = turma.Matriculas.Count;
            if (!jaMatriculado && ocupacao >= 5)
                throw new InvalidOperationException($"Turma {turma.Idioma} {turma.Numero} está lotada (máx. 5).");
        }

        //  Sincroniza matrículas: remove as que não estão mais desejadas e adiciona as novas
        var desejadasIds = turmas.Select(t => t.Id).ToHashSet();
        var atuaisIds    = aluno.Matriculas.Select(m => m.TurmaId).ToHashSet();

        // Remover matrículas que saíram
        var remover = aluno.Matriculas.Where(m => !desejadasIds.Contains(m.TurmaId)).ToList();
        _db.Matriculas.RemoveRange(remover);

        // Adicionar novas matrículas
        var adicionarIds = desejadasIds.Except(atuaisIds);
        foreach (var tid in adicionarIds)
        {
            _db.Matriculas.Add(new LanguageCourses.Domain.Entities.Matricula
            {
                AlunoId = aluno.Id,
                TurmaId = tid,
                DataMatricula = DateTime.UtcNow
            });
        }

        //  Persiste alterações
        await _db.SaveChangesAsync(ct);

        //  Recarrega as turmas do aluno para o retorno
        await _db.Entry(aluno).Collection(a => a.Matriculas).Query().Include(m => m.Turma).LoadAsync(ct);

        //  Monta DTO detalhado
        return new AlunoDetalheDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            Cpf = aluno.Cpf,
            Idade = aluno.Idade,
            Turmas = aluno.Matriculas.Select(m => new TurmaResumoDto
            {
                Id = m.Turma.Id,
                Idioma = m.Turma.Idioma,
                Numero = m.Turma.Numero,
                AnoLetivo = m.Turma.AnoLetivo
            })
            .OrderBy(t => t.Idioma)
            .ThenBy(t => t.Numero)
            .ToList()
        };
    }
}
