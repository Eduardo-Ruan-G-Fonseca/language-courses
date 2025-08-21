namespace LanguageCourses.Domain.Entities;

/// <summary>
/// Entidade que representa um aluno dentro do domínio do sistema de cursos de idiomas.
/// </summary>
/// <remarks>
/// <para>
/// Um aluno deve estar sempre vinculado a pelo menos uma matrícula (turma).  
/// A exclusão de um aluno deve ser bloqueada caso ainda possua matrículas ativas.
/// </para>
/// <para>
/// <strong>Regras de integridade:</strong>
/// <list type="bullet">
/// <item><description><see cref="Cpf"/> deve ser único e válido conforme cálculo oficial.</description></item>
/// <item><description><see cref="Email"/> deve ser único e válido.</description></item>
/// <item><description>Aluno deve estar matriculado em ≥ 1 turma.</description></item>
/// </list>
/// </para>
/// </remarks>
public class Aluno
{
    /// <summary>
    /// Identificador único do aluno (gerado pelo banco de dados).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome completo do aluno.
    /// </summary>
    public string Nome { get; set; } = default!;

    /// <summary>
    /// Endereço de e-mail do aluno (deve ser único e válido).
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// CPF do aluno (deve ser único e válido com dígitos verificadores).
    /// </summary>
    public string Cpf { get; set; } = default!;

    /// <summary>
    /// Idade do aluno (em anos).  
    /// Pode ser usada em regras de negócios específicas (ex.: restrição etária de turmas).
    /// </summary>
    public int Idade { get; set; }

    /// <summary>
    /// Relação de matrículas associadas ao aluno.  
    /// Cada matrícula vincula o aluno a uma <see cref="Turma"/>.
    /// </summary>
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}