using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

public record DeleteAlunoCommand(int Id) : IRequest<Unit>;