using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Turmas;

public class GetTurmaByIdHandler : IRequestHandler<GetTurmaByIdQuery, TurmaDto?>
{
    private readonly LanguageCoursesDbContext _db;
    public GetTurmaByIdHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<TurmaDto?> Handle(GetTurmaByIdQuery request, CancellationToken ct)
    {
        var t = await _db.Turmas.Include(x => x.Matriculas)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (t is null) return null;

        return new TurmaDto
        {
            Id = t.Id,
            Numero = t.Numero,
            Idioma = t.Idioma,
            AnoLetivo = t.AnoLetivo,
            VagasRestantes = 5 - t.Matriculas.Count
        };
    }
}