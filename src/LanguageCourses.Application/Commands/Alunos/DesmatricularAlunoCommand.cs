using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

public record DesmatricularAlunoCommand(int AlunoId, DesmatricularAlunoDto Matricula) : IRequest<AlunoDetalheDto>;