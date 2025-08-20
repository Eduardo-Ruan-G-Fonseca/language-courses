namespace LanguageCourses.Domain.Entities;

public class Matricula
{
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = default!;

    public int TurmaId { get; set; }
    public Turma Turma { get; set; } = default!;

    public DateTime DataMatricula { get; set; } = DateTime.UtcNow;
}