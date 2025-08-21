using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Queries.Alunos;

/// <summary>
/// Consulta responsável por obter os detalhes de um aluno pelo identificador.
/// </summary>
/// <param name="Id">Identificador único do aluno.</param>
/// <returns>
/// Um <see cref="AlunoDetalheDto"/> com os dados do aluno e suas turmas,
/// ou <see langword="null"/> caso não seja encontrado.
/// </returns>
public record GetAlunoByIdQuery(int Id) : IRequest<AlunoDetalheDto?>;