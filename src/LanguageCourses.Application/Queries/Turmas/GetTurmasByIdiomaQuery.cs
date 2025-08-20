using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Turmas;

public record GetTurmasByIdiomaQuery(string Idioma) : IRequest<List<TurmaDto>>;