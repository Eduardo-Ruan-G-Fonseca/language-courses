using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

public record MatricularAlunoCommand(int AlunoId, MatricularAlunoDto Matricula) : IRequest<AlunoDetalheDto>;