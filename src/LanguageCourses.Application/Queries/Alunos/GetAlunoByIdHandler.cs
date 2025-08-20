using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Alunos;

public class GetAlunoByIdHandler : IRequestHandler<GetAlunoByIdQuery, AlunoDetalheDto?>
{
    private readonly LanguageCoursesDbContext _db;
    public GetAlunoByIdHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<AlunoDetalheDto?> Handle(GetAlunoByIdQuery request, CancellationToken ct)
    {
        var aluno = await _db.Alunos
            .Include(a => a.Matriculas).ThenInclude(m => m.Turma)
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (aluno is null) return null;

        return new AlunoDetalheDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            Cpf = aluno.Cpf,
            Idade = aluno.Idade,
            Turmas = aluno.Matriculas.Select(m => new TurmaResumoDto
            {
                Id = m.Turma.Id, Idioma = m.Turma.Idioma, Numero = m.Turma.Numero, AnoLetivo = m.Turma.AnoLetivo
            }).OrderBy(t => t.Idioma).ThenBy(t => t.Numero).ToList()
        };
    }
}