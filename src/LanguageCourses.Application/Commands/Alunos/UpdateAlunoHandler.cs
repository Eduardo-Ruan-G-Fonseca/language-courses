using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LanguageCourses.Application.Commands.Alunos;

public class UpdateAlunoHandler : IRequestHandler<UpdateAlunoCommand, AlunoDetalheDto>
{
    private readonly LanguageCoursesDbContext _db;
    public UpdateAlunoHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<AlunoDetalheDto> Handle(UpdateAlunoCommand request, CancellationToken ct)
    {
        var aluno = await _db.Alunos
            .Include(a => a.Matriculas).ThenInclude(m => m.Turma)
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (aluno is null)
            throw new KeyNotFoundException("Aluno não encontrado.");

        var dto = request.Aluno;

        // Normaliza CPF (aceita com/sem máscara)
        var cpfDigits = Regex.Replace(dto.Cpf ?? "", "[^0-9]", "");

        // Unicidade amigável (email e cpf já normalizado)
        if (await _db.Alunos.AnyAsync(a => a.Id != aluno.Id && a.Email == dto.Email.Trim(), ct))
            throw new InvalidOperationException("E-mail já cadastrado.");
        if (await _db.Alunos.AnyAsync(a => a.Id != aluno.Id && a.Cpf == cpfDigits, ct))
            throw new InvalidOperationException("CPF já cadastrado.");

        aluno.Nome  = dto.Nome.Trim();
        aluno.Email = dto.Email.Trim();
        aluno.Cpf   = cpfDigits; // grava só dígitos
        aluno.Idade = dto.Idade;

        // Atualiza turmas: regra exige ≥ 1 turma
        if (dto.Turmas == null || dto.Turmas.Count == 0)
            throw new InvalidOperationException("Aluno deve permanecer em pelo menos 1 turma.");

        var refs = dto.Turmas.Select(t => new { Idioma = t.Idioma.Trim(), t.Numero }).ToList();
        var idiomas = refs.Select(r => r.Idioma).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var numeros = refs.Select(r => r.Numero).Distinct().ToList();

        var turmas = await _db.Turmas
            .Where(t => numeros.Contains(t.Numero) && idiomas.Contains(t.Idioma))
            .Include(t => t.Matriculas)
            .ToListAsync(ct);

        // Valida existência e capacidade
        foreach (var r in refs)
        {
            var turma = turmas.FirstOrDefault(t =>
                t.Numero == r.Numero &&
                string.Equals(t.Idioma, r.Idioma, StringComparison.OrdinalIgnoreCase));

            if (turma is null)
                throw new KeyNotFoundException($"Turma {r.Idioma} {r.Numero} não encontrada.");

            // Se aluno ainda não está nessa turma, checar capacidade
            var jaMatriculado = aluno.Matriculas.Any(m => m.TurmaId == turma.Id);
            var ocupacao = turma.Matriculas.Count;
            if (!jaMatriculado && ocupacao >= 5)
                throw new InvalidOperationException($"Turma {turma.Idioma} {turma.Numero} está lotada (máx. 5).");
        }

        // Sincroniza matrículas: remove as que saíram, adiciona as novas
        var desejadasIds = turmas.Select(t => t.Id).ToHashSet();
        var atuaisIds    = aluno.Matriculas.Select(m => m.TurmaId).ToHashSet();

        var remover = aluno.Matriculas.Where(m => !desejadasIds.Contains(m.TurmaId)).ToList();
        _db.Matriculas.RemoveRange(remover);

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

        await _db.SaveChangesAsync(ct);

        // Recarrega as turmas do aluno
        await _db.Entry(aluno).Collection(a => a.Matriculas).Query().Include(m => m.Turma).LoadAsync(ct);

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
            }).OrderBy(t => t.Idioma).ThenBy(t => t.Numero).ToList()
        };
    }
}
