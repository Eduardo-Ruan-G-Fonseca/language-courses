using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Alunos;

public class GetAlunosHandler : IRequestHandler<GetAlunosQuery, List<AlunoListItemDto>>
{
    private readonly LanguageCoursesDbContext _db;
    public GetAlunosHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<List<AlunoListItemDto>> Handle(GetAlunosQuery request, CancellationToken ct)
    {
        var list = await _db.Alunos
            .OrderBy(a => a.Nome)
            .Select(a => new AlunoListItemDto
            {
                Id = a.Id, Nome = a.Nome, Email = a.Email, Cpf = a.Cpf, Idade = a.Idade
            })
            .ToListAsync(ct);

        return list;
    }
}