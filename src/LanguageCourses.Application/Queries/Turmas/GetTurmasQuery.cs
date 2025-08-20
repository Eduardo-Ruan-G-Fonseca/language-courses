using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Turmas;

public record GetTurmasQuery() : IRequest<List<TurmaDto>>;