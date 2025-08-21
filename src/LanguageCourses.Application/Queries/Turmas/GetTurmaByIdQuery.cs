using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Turmas;

/// <summary>
/// Consulta responsável por obter os dados de uma turma pelo identificador.
/// </summary>
/// <param name="Id">Identificador único da turma.</param>
/// <returns>
/// Um <see cref="TurmaDto"/> com as informações da turma (incluindo vagas restantes),
/// ou <see langword="null"/> caso a turma não seja encontrada.
/// </returns>
public record GetTurmaByIdQuery(int Id) : IRequest<TurmaDto?>;