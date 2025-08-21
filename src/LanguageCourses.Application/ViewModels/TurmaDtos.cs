namespace LanguageCourses.Application.ViewModels;

/// <summary>
/// DTO utilizado para criação de uma nova turma.
/// </summary>
/// <remarks>
/// Contém os dados necessários para cadastrar uma turma,
/// incluindo número, idioma e ano letivo.
/// </remarks>
public class CreateTurmaDto
{
    /// <summary>
    /// Número identificador da turma (ex.: 101, 202).
    /// </summary>
    public int Numero { get; set; }

    /// <summary>
    /// Idioma da turma (ex.: Inglês, Espanhol, Francês).
    /// </summary>
    public string Idioma { get; set; } = "";

    /// <summary>
    /// Ano letivo da turma (ex.: "2025/1", "2025/2").
    /// </summary>
    public string AnoLetivo { get; set; } = "";
}

/// <summary>
/// DTO utilizado para atualização dos dados de uma turma existente.
/// </summary>
public class UpdateTurmaDto
{
    public int Numero { get; set; }
    public string Idioma { get; set; } = "";
    public string AnoLetivo { get; set; } = "";
}

/// <summary>
/// DTO completo de uma turma, usado em listagens ou consultas detalhadas.
/// Inclui número de vagas restantes.
/// </summary>
public class TurmaDto
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public string Idioma { get; set; } = "";
    public string AnoLetivo { get; set; } = "";

    /// <summary>
    /// Número de vagas restantes na turma (máximo de 5 alunos).
    /// </summary>
    public int VagasRestantes { get; set; }
}

/// <summary>
/// DTO de referência de turma.  
/// Utilizado em operações de matrícula/desmatrícula e vinculação de alunos.
/// </summary>
public class TurmaRefDto
{
    /// <summary>
    /// Idioma da turma.
    /// </summary>
    public string Idioma { get; set; } = "";

    /// <summary>
    /// Número identificador da turma dentro do idioma.
    /// </summary>
    public int Numero { get; set; }
}

/// <summary>
/// DTO resumido de turma, usado em consultas rápidas (ex.: turmas vinculadas a um aluno).
/// </summary>
public class TurmaResumoDto
{
    public int Id { get; set; }
    public string Idioma { get; set; } = "";
    public int Numero { get; set; }
    public string AnoLetivo { get; set; } = "";
}
