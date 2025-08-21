using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Handler responsável pelo processamento do <see cref="DeleteAlunoCommand"/>.
/// </summary>
/// <remarks>
/// Este handler implementa a exclusão de um aluno, garantindo as regras de negócio:
/// <list type="bullet">
/// <item><description>Verifica se o aluno existe; caso contrário, lança <see cref="KeyNotFoundException"/>.</description></item>
/// <item><description>Impede exclusão caso o aluno possua matrículas ativas, lançando <see cref="InvalidOperationException"/>.</description></item>
/// <item><description>Remove o aluno do banco de dados caso as validações sejam atendidas.</description></item>
/// </list>
/// </remarks>
public class DeleteAlunoHandler : IRequestHandler<DeleteAlunoCommand, Unit>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Construtor que injeta o <see cref="LanguageCoursesDbContext"/>.
    /// </summary>
    /// <param name="db">Contexto do Entity Framework responsável pelo acesso ao banco de dados.</param>
    public DeleteAlunoHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a exclusão de um aluno, garantindo as regras de integridade.
    /// </summary>
    /// <param name="request">Command com o identificador do aluno a ser excluído.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns><see cref="Unit"/> representando a conclusão da operação.</returns>
    /// <exception cref="KeyNotFoundException">Lançada se o aluno não for encontrado.</exception>
    /// <exception cref="InvalidOperationException">Lançada se o aluno possuir matrículas e não puder ser excluído.</exception>
    public async Task<Unit> Handle(DeleteAlunoCommand request, CancellationToken ct)
    {
        // Carrega aluno com suas matrículas
        var aluno = await _db.Alunos
            .Include(a => a.Matriculas)
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
