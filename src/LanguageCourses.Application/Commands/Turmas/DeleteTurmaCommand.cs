using MediatR;

namespace LanguageCourses.Application.Commands.Turmas;

public record DeleteTurmaCommand(int Id) : IRequest<Unit>;