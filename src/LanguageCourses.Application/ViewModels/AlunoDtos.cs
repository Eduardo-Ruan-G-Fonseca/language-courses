namespace LanguageCourses.Application.ViewModels;

/// <summary>
/// DTO utilizado para criação de um novo aluno.
/// </summary>
/// <remarks>
/// Contém os dados necessários para cadastrar um aluno, incluindo
/// a lista de turmas nas quais ele será matriculado.
/// </remarks>
public class CreateAlunoDto
{
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }

    /// <summary>
    /// Lista de turmas nas quais o aluno será matriculado na criação.
    /// </summary>
    public List<TurmaRefDto> Turmas { get; set; } = new();
}

/// <summary>
/// DTO utilizado para atualização dos dados de um aluno existente.
/// </summary>
public class UpdateAlunoDto
{
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }

    /// <summary>
    /// Lista de turmas atualizadas para o aluno.
    /// </summary>
    public List<TurmaRefDto> Turmas { get; set; } = new();
}

/// <summary>
/// DTO usado para listagem resumida de alunos (ex.: catálogo).
/// </summary>
public class AlunoListItemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }
}

/// <summary>
/// DTO detalhado de um aluno, incluindo as turmas vinculadas.
/// </summary>
public class AlunoDetalheDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }

    /// <summary>
    /// Lista de turmas em que o aluno está matriculado (resumidas).
    /// </summary>
    public List<TurmaResumoDto> Turmas { get; set; } = new();
}

/// <summary>
/// DTO para matrícula de um aluno em uma turma específica.
/// </summary>
public class MatricularAlunoDto
{
    /// <summary>
    /// Idioma da turma em que o aluno será matriculado.
    /// </summary>
    public string Idioma { get; set; } = "";

    /// <summary>
    /// Número identificador da turma dentro do idioma.
    /// </summary>
    public int Numero { get; set; }
}

/// <summary>
/// DTO para desmatrícula de um aluno de uma turma.
/// </summary>
public class DesmatricularAlunoDto
{
    /// <summary>
    /// Idioma da turma em que o aluno será desmatriculado.
    /// </summary>
    public string Idioma { get; set; } = "";

    /// <summary>
    /// Número identificador da turma dentro do idioma.
    /// </summary>
    public int Numero { get; set; }
}
