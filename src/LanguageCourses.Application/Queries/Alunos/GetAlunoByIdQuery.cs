using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Alunos;

public record GetAlunoByIdQuery(int Id) : IRequest<AlunoDetalheDto?>;