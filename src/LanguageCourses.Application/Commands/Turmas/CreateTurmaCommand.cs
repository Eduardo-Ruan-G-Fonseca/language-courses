using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Turmas;

public record CreateTurmaCommand(CreateTurmaDto Turma) : IRequest<TurmaDto>;