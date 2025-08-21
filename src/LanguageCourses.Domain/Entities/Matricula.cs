namespace LanguageCourses.Domain.Entities;

/// <summary>
/// Entidade que representa a matrícula de um <see cref="Aluno"/> em uma <see cref="Turma"/>.
/// </summary>
/// <remarks>
/// <para>
/// A matrícula é a entidade de junção (tabela associativa) no relacionamento N:N entre alunos e turmas.  
/// Cada registro vincula um único aluno a uma única turma.
/// </para>
/// <para>
/// <strong>Regras de negócio:</strong>
/// <list type="bullet">
/// <item><description>Um aluno não pode estar matriculado mais de uma vez na mesma turma.</description></item>
/// <item><description>A data de matrícula é definida automaticamente no momento da criação (<see cref="DataMatricula"/>).</description></item>
/// <item><description>A exclusão de uma matrícula deve respeitar a regra de que o aluno deve permanecer em ≥ 1 turma.</description></item>
/// </list>
/// </para>
/// </remarks>
public class Matricula
{
    /// <summary>
    /// Chave estrangeira para o aluno associado.
    /// </summary>
    public int AlunoId { get; set; }

    /// <summary>
    /// Navegação para o aluno desta matrícula.
    /// </summary>
    public Aluno Aluno { get; set; } = default!;

    /// <summary>
    /// Chave estrangeira para a turma associada.
    /// </summary>
    public int TurmaId { get; set; }

    /// <summary>
    /// Navegação para a turma desta matrícula.
    /// </summary>
    public Turma Turma { get; set; } = default!;

    /// <summary>
    /// Data em que a matrícula foi criada.  
    /// Valor padrão: <see cref="DateTime.UtcNow"/>.
    /// </summary>
    public DateTime DataMatricula { get; set; } = DateTime.UtcNow;
}