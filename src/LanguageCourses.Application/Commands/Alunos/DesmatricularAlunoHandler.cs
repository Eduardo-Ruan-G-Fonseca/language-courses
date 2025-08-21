using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Handler responsável pelo processamento do <see cref="DesmatricularAlunoCommand"/>.
/// </summary>
/// <remarks>
/// Este handler implementa a lógica de desmatrícula de um aluno em uma turma específica,
/// garantindo que as regras de negócio sejam respeitadas:
/// <list type="bullet">
/// <item><description>Verifica se o aluno existe; caso contrário, lança <see cref="KeyNotFoundException"/>.</description></item>
/// <item><description>Verifica se a turma existe; caso contrário, lança <see cref="KeyNotFoundException"/>.</description></item>
/// <item><description>Verifica se o aluno está matriculado na turma informada; caso contrário, lança <see cref="InvalidOperationException"/>.</description></item>
/// <item><description>Garante que o aluno continue matriculado em pelo menos uma turma após a operação; caso contrário, lança <see cref="InvalidOperationException"/>.</description></item>
/// </list>
/// </remarks>
public class DesmatricularAlunoHandler : IRequestHandler<DesmatricularAlunoCommand, AlunoDetalheDto>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Construtor que injeta o <see cref="LanguageCoursesDbContext"/>.
    /// </summary>
    public DesmatricularAlunoHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a desmatrícula de um aluno em uma turma, garantindo as regras de negócio.
    /// </summary>
    /// <param name="request">Command com o Id do aluno e a turma a ser removida.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>DTO detalhado do aluno atualizado com as turmas restantes.</returns>
    /// <exception cref="KeyNotFoundException">Se o aluno ou a turma não forem encontrados.</exception>
    /// <exception cref="InvalidOperationException">Se o aluno não estiver matriculado na turma informada ou se a desmatrícula o deixaria sem turmas.</exception>
    public async Task<AlunoDetalheDto> Handle(DesmatricularAlunoCommand request, CancellationToken ct)
    {
        //  Carrega aluno e suas matrículas
        var aluno = await _db.Alunos
            .Include(a => a.Matriculas).ThenInclude(m => m.Turma)
            .FirstOrDefaultAsync(a => a.Id == request.AlunoId, ct);

        if (aluno is null)
            throw new KeyNotFoundException("Aluno não encontrado.");

        var idioma = request.Matricula.Idioma.Trim();
        var numero = request.Matricula.Numero;

        //  Valida turma
        var turma = await _db.Turmas
            .FirstOrDefaultAsync(t => t.Numero == numero && 
                                      t.Idioma.ToLower() == idioma.ToLower(), ct);

        if (turma is null)
            throw new KeyNotFoundException($"Turma {idioma} {numero} não encontrada.");

        //  Verifica matrícula do aluno
        var mat = aluno.Matriculas.FirstOrDefault(m => m.TurmaId == turma.Id);
        if (mat is null)
            throw new InvalidOperationException("Aluno não está matriculado nessa turma.");

        //  Garante pelo menos 1 turma após a remoção
        if (aluno.Matriculas.Count <= 1)
            throw new InvalidOperationException("Aluno deve permanecer com pelo menos 1 turma.");

        //  Remove a matrícula
        _db.Matriculas.Remove(mat);
        await _db.SaveChangesAsync(ct);

        //  Atualiza navegação de matrículas
        await _db.Entry(aluno).Collection(a => a.Matriculas).Query().Include(m => m.Turma).LoadAsync(ct);

        //  Retorna DTO detalhado
        return new AlunoDetalheDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            Cpf = aluno.Cpf,
            Idade = aluno.Idade,
            Turmas = aluno.Matriculas
                .Select(m => new TurmaResumoDto
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
