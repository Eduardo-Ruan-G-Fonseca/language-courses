namespace LanguageCourses.Domain.Entities;

public class Aluno
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Cpf { get; set; } = default!;
    public int Idade { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}