using System.ComponentModel.DataAnnotations.Schema;

namespace LanguageCourses.Domain.Entities;

/// <summary>
/// Entidade que representa uma turma de curso de idiomas.
/// </summary>
/// <remarks>
/// <para>
/// Cada turma é identificada por uma combinação de <see cref="Idioma"/> + <see cref="Numero"/> + <see cref="AnoLetivo"/>.  
/// A turma possui um limite máximo de 5 alunos matriculados.
/// </para>
/// <para>
/// <strong>Regras de negócio:</strong>
/// <list type="bullet">
/// <item><description>Uma turma não pode ter mais que 5 alunos (controlado pela propriedade <see cref="VagasRestantes"/>).</description></item>
/// <item><description>Não deve existir duplicidade de turmas com a mesma combinação de Idioma, Número e Ano Letivo.</description></item>
/// <item><description>A exclusão da turma deve ser bloqueada caso existam alunos matriculados.</description></item>
/// </list>
/// </para>
/// </remarks>
public class Turma
{
    /// <summary>
    /// Identificador único da turma (gerado pelo banco de dados).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Número identificador da turma dentro de um idioma (ex.: 101, 202).
    /// </summary>
    public int Numero { get; set; }

    /// <summary>
    /// Idioma da turma (ex.: Inglês, Espanhol, Francês).
    /// </summary>
    public string Idioma { get; set; } = default!;

    /// <summary>
    /// Ano letivo da turma (ex.: "2025/1", "2025/2").
    /// </summary>
    public string AnoLetivo { get; set; } = default!;

    /// <summary>
    /// Coleção de matrículas associadas a esta turma.  
    /// Representa os alunos vinculados.
    /// </summary>
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    /// <summary>
    /// Calcula dinamicamente o número de vagas restantes.  
    /// Limite máximo fixado em 5 alunos por turma.
    /// </summary>
    [NotMapped]
    public int VagasRestantes => 5 - Matriculas.Count;
}