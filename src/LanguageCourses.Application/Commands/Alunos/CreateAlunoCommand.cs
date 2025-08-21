using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Command responsável pela criação de um novo aluno.
/// </summary>
/// <remarks>
/// Este command encapsula os dados necessários para criar um aluno e
/// delega ao respectivo handler (<c>CreateAlunoHandler</c>) a lógica de validação e persistência.
///
/// <para><strong>Regras de negócio aplicadas no fluxo:</strong></para>
/// <list type="bullet">
/// <item><description>CPF deve ser válido (cálculo de dígitos verificadores).</description></item>
/// <item><description>E-mail deve ser válido e único.</description></item>
/// <item><description>Aluno deve estar matriculado em pelo menos 1 turma.</description></item>
/// <item><description>Não deve haver turmas duplicadas para o mesmo aluno.</description></item>
/// </list>
/// </remarks>
/// <param name="Aluno">DTO com os dados necessários para criação do aluno.</param>
/// <returns><see cref="AlunoDetalheDto"/> com informações completas do aluno criado.</returns>
public record CreateAlunoCommand(CreateAlunoDto Aluno) : IRequest<AlunoDetalheDto>;