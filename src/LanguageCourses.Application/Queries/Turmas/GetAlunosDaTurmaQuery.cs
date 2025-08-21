using MediatR;

namespace LanguageCourses.Application.Queries.Turmas;

/// <summary>
/// Query para buscar todos os alunos de uma turma específica,
/// identificada pelo idioma e número.
/// </summary>
/// <param name="Idioma">Idioma da turma (ex: "Inglês").</param>
/// <param name="Numero">Número identificador da turma.</param>
public record GetAlunosDaTurmaQuery(string Idioma, int Numero) 
    : IRequest<List<AlunoTurmaListItem>>;

/// <summary>
/// DTO que representa um aluno dentro de uma turma.
/// </summary>
public class AlunoTurmaListItem
{
    /// <summary>
    /// Identificador único do aluno.
    /// </summary>
    public int AlunoId { get; set; }

    /// <summary>
    /// Nome completo do aluno.
    /// </summary>
    public string Nome { get; set; } = "";

    /// <summary>
    /// Endereço de e-mail do aluno.
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// CPF do aluno.
    /// </summary>
    public string Cpf { get; set; } = "";

    /// <summary>
    /// Idade do aluno.
    /// </summary>
    public int Idade { get; set; }
}