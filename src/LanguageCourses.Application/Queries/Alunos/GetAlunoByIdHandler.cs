using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Alunos;

/// <summary>
/// Manipulador da consulta <see cref="GetAlunoByIdQuery"/>.
/// Responsável por carregar um aluno por identificador e projetar o resultado em <see cref="AlunoDetalheDto"/>.
/// </summary>
/// <remarks>
/// Esta consulta retorna <see langword="null"/> quando o aluno não é encontrado.
/// Carrega as matrículas e suas respectivas turmas para compor o retorno detalhado.
/// </remarks>
public class GetAlunoByIdHandler : IRequestHandler<GetAlunoByIdQuery, AlunoDetalheDto?>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="GetAlunoByIdHandler"/>.
    /// </summary>
    /// <param name="db">Contexto de dados da aplicação.</param>
    public GetAlunoByIdHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a consulta para obter um aluno por identificador.
    /// </summary>
    /// <param name="request">Consulta contendo o identificador do aluno.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Um <see cref="AlunoDetalheDto"/> com os dados do aluno e suas turmas,
    /// ou <see langword="null"/> caso o aluno não seja encontrado.
    /// </returns>
    public async Task<AlunoDetalheDto?> Handle(GetAlunoByIdQuery request, CancellationToken ct)
    {
        var aluno = await _db.Alunos
            .Include(a => a.Matriculas).ThenInclude(m => m.Turma)
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (aluno is null) return null;

        return new AlunoDetalheDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            Cpf = aluno.Cpf,
            Idade = aluno.Idade,
            Turmas = aluno.Matriculas
                .Select(m => new TurmaResumoDto
                {
                    Id = m.Turma.Id,
                    Idioma = m.Turma.Idioma,
                    Numero = m.Turma.Numero,
                    AnoLetivo = m.Turma.AnoLetivo
                })
                .OrderBy(t => t.Idioma)
                .ThenBy(t => t.Numero)
                .ToList()
        };
    }
}
