using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Alunos;

public record GetAlunosQuery() : IRequest<List<AlunoListItemDto>>;