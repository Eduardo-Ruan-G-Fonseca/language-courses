using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Turmas;

public class GetAlunosDaTurmaHandler : IRequestHandler<GetAlunosDaTurmaQuery, List<AlunoTurmaListItem>>
{
    private readonly LanguageCoursesDbContext _db;
    public GetAlunosDaTurmaHandler(LanguageCoursesDbContext db) => _db = db;

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