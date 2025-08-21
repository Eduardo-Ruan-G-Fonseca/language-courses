using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Turmas;

/// <summary>
/// Manipulador do comando <see cref="DeleteTurmaCommand"/>.
/// </summary>
/// <remarks>
/// Responsável por processar a exclusão de uma turma no sistema.  
/// A exclusão somente é permitida caso a turma exista e não possua
/// alunos matriculados associados.
/// </remarks>
public class DeleteTurmaHandler : IRequestHandler<DeleteTurmaCommand, Unit>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="DeleteTurmaHandler"/>.
    /// </summary>
    /// <param name="db">Contexto de persistência da aplicação.</param>
    public DeleteTurmaHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a remoção da turma informada no comando.
    /// </summary>
    /// <param name="request">Comando contendo o identificador da turma a ser removida.</param>
    /// <param name="ct">Token de cancelamento da operação.</param>
    /// <returns>Um valor <see cref="Unit"/> indicando a conclusão da operação.</returns>
    /// <exception cref="KeyNotFoundException">Lançada quando a turma não é encontrada.</exception>
    /// <exception cref="InvalidOperationException">Lançada quando a turma possui alunos matriculados.</exception>
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