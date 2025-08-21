using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Turmas;

/// <summary>
/// Consulta para obter todas as turmas de um determinado idioma.
/// </summary>
/// <param name="Idioma">Idioma utilizado como filtro (comparação case-insensitive).</param>
/// <returns>
/// Lista de <see cref="TurmaDto"/> correspondente ao idioma informado.
/// </returns>
public record GetTurmasByIdiomaQuery(string Idioma) : IRequest<List<TurmaDto>>;