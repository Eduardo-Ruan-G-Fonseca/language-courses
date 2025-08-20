using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using LanguageCourses.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Turmas;

public class CreateTurmaHandler : IRequestHandler<CreateTurmaCommand, TurmaDto>
{
    private readonly LanguageCoursesDbContext _db;
    public CreateTurmaHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<TurmaDto> Handle(CreateTurmaCommand request, CancellationToken ct)
    {
        var idioma = request.Turma.Idioma.Trim();
        var numero = request.Turma.Numero;
        
        var exists = await _db.Turmas.AnyAsync(t =>
            t.Numero == numero &&
            t.Idioma.ToLower() == idioma.ToLower(), ct);

        if (exists)
            throw new InvalidOperationException($"Já existe uma turma {idioma} {numero}.");

        var turma = new Turma
        {
            Numero = numero,
            Idioma = idioma,
            AnoLetivo = request.Turma.AnoLetivo.Trim()
        };

        _db.Turmas.Add(turma);
        await _db.SaveChangesAsync(ct);

        return new TurmaDto
        {
            Id = turma.Id,
            Numero = turma.Numero,
            Idioma = turma.Idioma,
            AnoLetivo = turma.AnoLetivo,
            VagasRestantes = 5
        };
    }
}