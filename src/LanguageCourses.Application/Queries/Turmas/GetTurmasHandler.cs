using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Turmas;

public class GetTurmasHandler : IRequestHandler<GetTurmasQuery, List<TurmaDto>>
{
    private readonly LanguageCoursesDbContext _db;
    public GetTurmasHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<List<TurmaDto>> Handle(GetTurmasQuery request, CancellationToken ct)
    {
        var list = await _db.Turmas
            .Include(t => t.Matriculas)
            .OrderBy(t => t.Idioma).ThenBy(t => t.Numero)
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