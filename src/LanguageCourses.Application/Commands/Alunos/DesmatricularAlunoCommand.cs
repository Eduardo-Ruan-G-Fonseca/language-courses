using LanguageCourses.Application.ViewModels;
using MediatR;

namespace LanguageCourses.Application.Commands.Alunos;

/// <summary>
/// Command responsável por desmatricular um aluno de uma turma específica.
/// </summary>
/// <remarks>
/// Este command encapsula os dados necessários para remover a matrícula de um aluno
/// em uma turma (definida por Idioma + Número).
///
/// <para><strong>Regras de negócio aplicadas no fluxo:</strong></para>
/// <list type="bullet">
/// <item><description>O aluno deve permanecer matriculado em pelo menos 1 turma após a operação.</description></item>
/// <item><description>O aluno e a turma devem existir; caso contrário, lançar exceção apropriada.</description></item>
/// <item><description>Se o aluno não estiver matriculado na turma informada, lançar exceção.</description></item>
/// </list>
/// </remarks>
/// <param name="AlunoId">Identificador único do aluno.</param>
/// <param name="Matricula">DTO com Idioma e Número da turma a ser removida.</param>
/// <returns><see cref="AlunoDetalheDto"/> atualizado com as turmas restantes do aluno.</returns>
public record DesmatricularAlunoCommand(int AlunoId, DesmatricularAlunoDto Matricula) : IRequest<AlunoDetalheDto>;