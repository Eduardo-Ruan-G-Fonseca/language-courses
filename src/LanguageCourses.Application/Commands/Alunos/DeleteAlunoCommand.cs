using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Command responsável por excluir um aluno do sistema.
/// </summary>
/// <remarks>
/// Este command encapsula o identificador de um aluno a ser excluído e delega
/// ao respectivo handler (<c>DeleteAlunoHandler</c>) a lógica de exclusão.
/// 
/// <para><strong>Regras de negócio:</strong></para>
/// <list type="bullet">
/// <item><description>Um aluno não pode ser excluído caso esteja matriculado em alguma turma.</description></item>
/// <item><description>A exclusão deve ser realizada em cascata para registros de apoio, caso permitido.</description></item>
/// <item><description>Se o aluno não existir, deve ser retornado erro de recurso não encontrado.</description></item>
/// </list>
/// </remarks>
/// <param name="Id">Identificador único do aluno a ser excluído.</param>
/// <returns><see cref="Unit"/> indicando conclusão da operação.</returns>
public record DeleteAlunoCommand(int Id) : IRequest<Unit>;