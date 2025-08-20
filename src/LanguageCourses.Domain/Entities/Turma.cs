using System.ComponentModel.DataAnnotations.Schema;

namespace LanguageCourses.Domain.Entities;

public class Turma
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public string Idioma { get; set; } = default!;
    public string AnoLetivo { get; set; } = default!;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    [NotMapped]
    public int VagasRestantes => 5 - Matriculas.Count;
}