using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Alunos;

/// <summary>
/// Consulta para obter a lista de alunos em formato resumido.
/// </summary>
/// <remarks>
/// Essa query retorna uma coleção de <see cref="AlunoListItemDto"/>, 
/// contendo apenas informações básicas de identificação (Id, Nome, Email, CPF e Idade).
/// </remarks>
public record GetAlunosQuery() : IRequest<List<AlunoListItemDto>>;