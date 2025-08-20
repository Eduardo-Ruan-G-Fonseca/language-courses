using MediatR;

namespace LanguageCourses.Application.Queries.Turmas;

public record GetAlunosDaTurmaQuery(string Idioma, int Numero) : IRequest<List<AlunoTurmaListItem>>;

public class AlunoTurmaListItem
{
    public int AlunoId { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public string Cpf { get; set; } = "";
    public int Idade { get; set; }
}