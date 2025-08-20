using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Turmas;

public class DeleteTurmaHandler : IRequestHandler<DeleteTurmaCommand, Unit>
{
    private readonly LanguageCoursesDbContext _db;
    public DeleteTurmaHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteTurmaCommand request, CancellationToken ct)
    {
        var turma = await _db.Turmas
            .Include(t => t.Matriculas)
            .FirstOrDefaultAsync(t => t.Id == request.Id, ct);

        if (turma is null)
            throw new KeyNotFoundException("Turma não encontrada.");

        if (turma.Matriculas.Any())
            throw new InvalidOperationException("Turma não pode ser excluída pois possui alunos.");

        _db.Turmas.Remove(turma);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}