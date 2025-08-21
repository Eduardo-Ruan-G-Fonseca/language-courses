using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Alunos;

/// <summary>
/// Manipulador da consulta <see cref="GetAlunosComTurmasQuery"/>.
/// Retorna a lista de alunos com suas turmas associadas, projetada em <see cref="AlunoDetalheDto"/>.
/// </summary>
/// <remarks>
/// Carrega as matrículas e respectivas turmas via <see cref="EntityFrameworkQueryableExtensions.Include{TEntity, TProperty}(IQueryable{TEntity}, System.Linq.Expressions.Expression{System.Func{TEntity, TProperty}})"/>
/// e ordena os alunos por nome para uma resposta estável e previsível.
/// </remarks>
public class GetAlunosComTurmasHandler : IRequestHandler<GetAlunosComTurmasQuery, List<AlunoDetalheDto>>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="GetAlunosComTurmasHandler"/>.
    /// </summary>
    /// <param name="db">Contexto de dados da aplicação.</param>
    public GetAlunosComTurmasHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a consulta para obter todos os alunos com suas turmas.
    /// </summary>
    /// <param name="request">Consulta sem parâmetros.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Lista de <see cref="AlunoDetalheDto"/> contendo dados do aluno e suas turmas.
    /// </returns>
    public async Task<List<AlunoDetalheDto>> Handle(GetAlunosComTurmasQuery request, CancellationToken ct)
    {
        var alunos = await _db.Alunos
            .Include(a => a.Matriculas)
                .ThenInclude(m => m.Turma)
            .OrderBy(a => a.Nome)
            .ToListAsync(ct);

        return alunos.Select(a => new AlunoDetalheDto
        {
            Id = a.Id,
            Nome = a.Nome,
            Email = a.Email,
            Cpf = a.Cpf,
            Idade = a.Idade,
            Turmas = a.Matriculas
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
        }).ToList();
    }
}
