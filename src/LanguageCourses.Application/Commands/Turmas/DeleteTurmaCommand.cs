using MediatR;

namespace LanguageCourses.Application.Commands.Turmas;

/// <summary>
/// Comando para exclusão de uma turma existente.
/// </summary>
/// <remarks>
/// Este comando é utilizado para solicitar a remoção de uma turma
/// específica a partir do seu identificador único.
/// 
/// O processamento do comando é realizado pelo handler associado,
/// que deve verificar a existência da turma e removê-la do banco.
/// </remarks>
/// <param name="Id">Identificador único da turma a ser removida.</param>
public record DeleteTurmaCommand(int Id) : IRequest<Unit>;