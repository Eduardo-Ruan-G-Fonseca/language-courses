using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Alunos;

/// <summary>
/// Consulta para obter a lista de alunos com suas respectivas turmas.
/// </summary>
/// <remarks>
/// Retorna uma coleção de <see cref="AlunoDetalheDto"/> já contendo os dados
/// do aluno e as turmas em que está matriculado.
/// </remarks>
public record GetAlunosComTurmasQuery() : IRequest<List<AlunoDetalheDto>>;