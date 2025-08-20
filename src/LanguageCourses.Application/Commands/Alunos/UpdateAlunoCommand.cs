using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

public record UpdateAlunoCommand(int Id, UpdateAlunoDto Aluno) : IRequest<AlunoDetalheDto>;