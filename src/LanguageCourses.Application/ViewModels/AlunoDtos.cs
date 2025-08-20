namespace LanguageCourses.Application.ViewModels;

public class CreateAlunoDto
{
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }
    public List<TurmaRefDto> Turmas { get; set; } = new();
}

public class UpdateAlunoDto
{
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }
    public List<TurmaRefDto> Turmas { get; set; } = new();
}

public class AlunoListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }
}

public class AlunoDetalheDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }
    public List<TurmaResumoDto> Turmas { get; set; } = new();
}

public class MatricularAlunoDto
{
    public string Idioma { get; set; } = "";
    public int Numero { get; set; }
}

public class DesmatricularAlunoDto
{
    public string Idioma { get; set; } = "";
    public int Numero { get; set; }
}