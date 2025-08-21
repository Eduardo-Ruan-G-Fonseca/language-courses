using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Turmas;

/// <summary>
/// Manipulador da consulta <see cref="GetTurmasByIdiomaQuery"/>.
/// Retorna as turmas de um idioma específico, projetadas em <see cref="TurmaDto"/>.
/// </summary>
/// <remarks>
/// Carrega as matrículas para calcular <c>VagasRestantes</c> (limite de 5 alunos por turma).  
/// A comparação de idioma é case-insensitive e o resultado é ordenado por número da turma.
/// </remarks>
public class GetTurmasByIdiomaHandler : IRequestHandler<GetTurmasByIdiomaQuery, List<TurmaDto>>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="GetTurmasByIdiomaHandler"/>.
    /// </summary>
    /// <param name="db">Contexto de dados da aplicação.</param>
    public GetTurmasByIdiomaHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a consulta para obter as turmas filtradas por idioma.
    /// </summary>
    /// <param name="request">Consulta contendo o idioma a ser filtrado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de <see cref="TurmaDto"/> correspondentes ao idioma informado.</returns>
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