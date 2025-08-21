using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Turmas;

/// <summary>
/// Manipulador da consulta <see cref="GetTurmaByIdQuery"/>.
/// Recupera uma turma pelo identificador e projeta o resultado em <see cref="TurmaDto"/>.
/// </summary>
/// <remarks>
/// Carrega as matrículas para calcular dinamicamente <c>VagasRestantes</c> (limite de 5 alunos por turma).
/// Retorna <see langword="null"/> caso a turma não seja encontrada.
/// </remarks>
public class GetTurmaByIdHandler : IRequestHandler<GetTurmaByIdQuery, TurmaDto?>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="GetTurmaByIdHandler"/>.
    /// </summary>
    /// <param name="db">Contexto de dados da aplicação.</param>
    public GetTurmaByIdHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a consulta para obter uma turma pelo seu identificador.
    /// </summary>
    /// <param name="request">Consulta contendo o identificador da turma.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Um <see cref="TurmaDto"/> com os dados da turma e o número de vagas restantes,
    /// ou <see langword="null"/> se a turma não for encontrada.
    /// </returns>
    public async Task<TurmaDto?> Handle(GetTurmaByIdQuery request, CancellationToken ct)
    {
        var t = await _db.Turmas
            .Include(x => x.Matriculas)
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