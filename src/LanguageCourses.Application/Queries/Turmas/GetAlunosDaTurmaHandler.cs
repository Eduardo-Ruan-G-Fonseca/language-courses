using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Turmas;

/// <summary>
/// Handler responsável por processar a consulta de alunos pertencentes a uma turma.
/// </summary>
public class GetAlunosDaTurmaHandler : IRequestHandler<GetAlunosDaTurmaQuery, List<AlunoTurmaListItem>>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância do handler.
    /// </summary>
    /// <param name="db">Contexto de banco de dados da aplicação.</param>
    public GetAlunosDaTurmaHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Manipula a consulta para obter os alunos de uma turma específica, 
    /// identificada pelo idioma e número da turma.
    /// </summary>
    /// <param name="request">Dados da turma (idioma e número).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de alunos da turma, ordenados por nome.</returns>
    /// <exception cref="KeyNotFoundException">Lançada caso a turma não seja encontrada.</exception>
    public async Task<List<AlunoTurmaListItem>> Handle(GetAlunosDaTurmaQuery request, CancellationToken ct)
    {
        var idioma = request.Idioma.Trim();
        var numero = request.Numero;

        var turma = await _db.Turmas
            .FirstOrDefaultAsync(t => t.Numero == numero && t.Idioma.ToLower() == idioma.ToLower(), ct);

        if (turma is null)
            throw new KeyNotFoundException($"Turma {idioma} {numero} não encontrada.");

        var alunos = await _db.Matriculas
            .Where(m => m.TurmaId == turma.Id)
            .Select(m => new AlunoTurmaListItem
            {
                AlunoId = m.AlunoId,
                Nome = m.Aluno.Nome,
                Email = m.Aluno.Email,
                Cpf = m.Aluno.Cpf,
                Idade = m.Aluno.Idade
            })
            .OrderBy(x => x.Nome)
            .ToListAsync(ct);

        return alunos;
    }
}
