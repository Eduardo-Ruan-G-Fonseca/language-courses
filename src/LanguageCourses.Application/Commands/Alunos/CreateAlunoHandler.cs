using LanguageCourses.Application.ViewModels;
using LanguageCourses.Domain.Entities;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Handler responsável pelo processamento do <see cref="CreateAlunoCommand"/>.
/// </summary>
/// <remarks>
/// <para>
/// Implementa a lógica de criação de um aluno, garantindo as regras de negócio:
/// </para>
/// <list type="bullet">
/// <item><description>Normalização e verificação de unicidade de CPF e E-mail.</description></item>
/// <item><description>Validação da existência das turmas informadas.</description></item>
/// <item><description>Validação de capacidade (máx. 5 alunos por turma).</description></item>
/// <item><description>Criação do aluno e vínculo com turmas via entidade <see cref="Matricula"/>.</description></item>
/// </list>
/// </remarks>
public class CreateAlunoHandler : IRequestHandler<CreateAlunoCommand, AlunoDetalheDto>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Construtor que injeta o <see cref="LanguageCoursesDbContext"/>.
    /// </summary>
    public CreateAlunoHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a criação do aluno e suas matrículas.
    /// </summary>
    /// <param name="request">Command contendo o DTO do aluno a ser criado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>DTO detalhado do aluno criado.</returns>
    /// <exception cref="InvalidOperationException">Se e-mail ou CPF já estiverem cadastrados, ou se a turma estiver lotada.</exception>
    /// <exception cref="KeyNotFoundException">Se alguma turma informada não existir.</exception>
    public async Task<AlunoDetalheDto> Handle(CreateAlunoCommand request, CancellationToken ct)
    {
        var dto = request.Aluno;

        //  Normaliza CPF (remove caracteres não numéricos, aceita com/sem máscara)
        var cpfDigits = Regex.Replace(dto.Cpf ?? "", "[^0-9]", "");

        //  Validação de unicidade amigável (email/cpf)
        if (await _db.Alunos.AnyAsync(a => a.Email == dto.Email.Trim(), ct))
            throw new InvalidOperationException("E-mail já cadastrado.");
        if (await _db.Alunos.AnyAsync(a => a.Cpf == cpfDigits, ct))
            throw new InvalidOperationException("CPF já cadastrado.");

        //  Carrega turmas por (idioma, numero) e valida existência + capacidade
        var refs = dto.Turmas.Select(t => new { Idioma = t.Idioma.Trim(), t.Numero }).ToList();
        var idiomas = refs.Select(r => r.Idioma).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var numeros = refs.Select(r => r.Numero).Distinct().ToList();

        var turmas = await _db.Turmas
            .Where(t => numeros.Contains(t.Numero) && idiomas.Contains(t.Idioma))
            .Include(t => t.Matriculas)
            .ToListAsync(ct);

        foreach (var r in refs)
        {
            var turma = turmas.FirstOrDefault(t =>
                t.Numero == r.Numero &&
                string.Equals(t.Idioma, r.Idioma, StringComparison.OrdinalIgnoreCase));

            if (turma is null)
                throw new KeyNotFoundException($"Turma {r.Idioma} {r.Numero} não encontrada.");

            if (turma.Matriculas.Count >= 5)
                throw new InvalidOperationException($"Turma {turma.Idioma} {turma.Numero} está lotada (máx. 5).");
        }

        //  Cria o aluno
        var aluno = new Aluno
        {
            Nome  = dto.Nome.Trim(),
            Email = dto.Email.Trim(),
            Cpf   = cpfDigits, // grava somente os dígitos
            Idade = dto.Idade
        };

        _db.Alunos.Add(aluno);

        //  Cria as matrículas
        foreach (var r in refs)
        {
            var turma = turmas.First(t =>
                t.Numero == r.Numero &&
                string.Equals(t.Idioma, r.Idioma, StringComparison.OrdinalIgnoreCase));

            _db.Matriculas.Add(new Matricula
            {
                Aluno = aluno,
                Turma = turma,
                DataMatricula = DateTime.UtcNow
            });
        }

        //  Persiste alterações
        await _db.SaveChangesAsync(ct);

        //  Retorna DTO detalhado
        return new AlunoDetalheDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            Cpf = aluno.Cpf,
            Idade = aluno.Idade,
            Turmas = turmas.Select(t => new TurmaResumoDto
            {
                Id = t.Id,
                Idioma = t.Idioma,
                Numero = t.Numero,
                AnoLetivo = t.AnoLetivo
            })
            .OrderBy(t => t.Idioma)
            .ThenBy(t => t.Numero)
            .ToList()
        };
    }
}
