using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Comando responsável por atualizar os dados de um aluno existente.
/// </summary>
/// <param name="Id">Identificador único do aluno que será atualizado.</param>
/// <param name="Aluno">Objeto com os dados atualizados do aluno.</param>
/// <remarks>
/// Esse comando segue o padrão CQRS e é processado por um handler específico
/// (UpdateAlunoHandler). Ele retorna um <see cref="AlunoDetalheDto"/>,
/// contendo os dados atualizados do aluno após a operação.
/// </remarks>
public record UpdateAlunoCommand(int Id, UpdateAlunoDto Aluno) : IRequest<AlunoDetalheDto>;