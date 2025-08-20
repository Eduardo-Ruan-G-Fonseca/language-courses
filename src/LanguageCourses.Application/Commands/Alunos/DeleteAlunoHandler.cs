using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Alunos;

public class DeleteAlunoHandler : IRequestHandler<DeleteAlunoCommand, Unit>
{
    private readonly LanguageCoursesDbContext _db;
    public DeleteAlunoHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<Unit> Handle(DeleteAlunoCommand request, CancellationToken ct)
    {
        var aluno = await _db.Alunos.Include(a => a.Matriculas)
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (aluno is null)
            throw new KeyNotFoundException("Aluno não encontrado.");

        if (aluno.Matriculas.Any())
            throw new InvalidOperationException("Aluno não pode ser excluído pois está associado a turma(s).");

        _db.Alunos.Remove(aluno);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}