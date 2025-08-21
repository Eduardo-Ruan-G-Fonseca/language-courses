using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Turmas;

/// <summary>
/// Consulta para obter todas as turmas cadastradas.
/// </summary>
/// <remarks>
/// Retorna uma lista de <see cref="TurmaDto"/> representando as turmas
/// disponíveis no sistema.
/// </remarks>
public record GetTurmasQuery() : IRequest<List<TurmaDto>>;