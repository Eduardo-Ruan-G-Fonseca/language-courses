namespace LanguageCourses.Application.ViewModels;

public class CreateTurmaDto
{
    public int Numero { get; set; }
    public string Idioma { get; set; } = "";
    public string AnoLetivo { get; set; } = "";
}

public class UpdateTurmaDto
{
    public int Numero { get; set; }
    public string Idioma { get; set; } = "";
    public string AnoLetivo { get; set; } = "";
}

public class TurmaDto
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public string Idioma { get; set; } = "";
    public string AnoLetivo { get; set; } = "";
    public int VagasRestantes { get; set; }
}

public class TurmaRefDto
{
    public string Idioma { get; set; } = "";
    public int Numero { get; set; }
}

public class TurmaResumoDto
{
    public int Id { get; set; }
    public string Idioma { get; set; } = "";
    public int Numero { get; set; }
    public string AnoLetivo { get; set; } = "";
}