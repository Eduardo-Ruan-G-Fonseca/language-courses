using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Turmas;

/// <summary>
/// Manipulador da consulta <see cref="GetTurmasQuery"/>.
/// Retorna todas as turmas projetadas em <see cref="TurmaDto"/>, incluindo o cálculo de vagas restantes.
/// </summary>
/// <remarks>
/// As turmas são carregadas com suas matrículas para permitir o cálculo de <c>VagasRestantes</c>
/// (limite de 5 alunos por turma). O resultado é ordenado por idioma e, em seguida, por número.
/// </remarks>
public class GetTurmasHandler : IRequestHandler<GetTurmasQuery, List<TurmaDto>>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="GetTurmasHandler"/>.
    /// </summary>
    /// <param name="db">Contexto de dados da aplicação.</param>
    public GetTurmasHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a consulta para obter todas as turmas.
    /// </summary>
    /// <param name="request">Consulta sem parâmetros.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de <see cref="TurmaDto"/> ordenada por idioma e número.</returns>
    public async Task<List<TurmaDto>> Handle(GetTurmasQuery request, CancellationToken ct)
    {
        var list = await _db.Turmas
            .Include(t => t.Matriculas)
            .OrderBy(t => t.Idioma)
            .ThenBy(t => t.Numero)
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