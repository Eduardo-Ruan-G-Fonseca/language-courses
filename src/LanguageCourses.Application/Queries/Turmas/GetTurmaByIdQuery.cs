using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Turmas;

public record GetTurmaByIdQuery(int Id) : IRequest<TurmaDto?>;