using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Turmas;

public class GetTurmasByIdiomaHandler : IRequestHandler<GetTurmasByIdiomaQuery, List<TurmaDto>>
{
    private readonly LanguageCoursesDbContext _db;
    public GetTurmasByIdiomaHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<List<TurmaDto>> Handle(GetTurmasByIdiomaQuery request, CancellationToken ct)
    {
        var idioma = request.Idioma.Trim();

        var list = await _db.Turmas
            .Include(t => t.Matriculas)
            .Where(t => t.Idioma.ToLower() == idioma.ToLower())
            .OrderBy(t => t.Numero)
            .ToListAsync(ct);

        return list.Select(t => new TurmaDto
        {
            Id = t.Id,
            Numero = t.Numero,
            Idioma = t.Idioma,
            AnoLetivo = t.AnoLetivo,
            VagasRestantes = 5 - t.Matriculas.Count
        }).ToList();
    }
}