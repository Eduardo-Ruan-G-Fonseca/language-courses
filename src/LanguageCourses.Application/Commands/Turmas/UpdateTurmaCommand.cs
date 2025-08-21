using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Turmas;

/// <summary>
/// Comando para atualização de uma turma existente.
/// </summary>
/// <param name="Id">Identificador único da turma a ser atualizada.</param>
/// <param name="Turma">Objeto contendo os novos dados da turma.</param>
public record UpdateTurmaCommand(int Id, UpdateTurmaDto Turma) : IRequest<TurmaDto>;