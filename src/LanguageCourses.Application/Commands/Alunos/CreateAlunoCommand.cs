using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

public record CreateAlunoCommand(CreateAlunoDto Aluno) : IRequest<AlunoDetalheDto>;