using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Queries.Alunos;

/// <summary>
/// Manipulador da consulta <see cref="GetAlunosQuery"/>.
/// Retorna uma listagem resumida de alunos projetada em <see cref="AlunoListItemDto"/>.
/// </summary>
/// <remarks>
/// Executa projeção direta no banco de dados para eficiência, ordenando por nome.
/// Não carrega navegações, pois o objetivo é apenas a listagem resumida.
/// </remarks>
public class GetAlunosHandler : IRequestHandler<GetAlunosQuery, List<AlunoListItemDto>>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="GetAlunosHandler"/>.
    /// </summary>
    /// <param name="db">Contexto de dados da aplicação.</param>
    public GetAlunosHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Executa a consulta de listagem de alunos.
    /// </summary>
    /// <param name="request">Consulta sem parâmetros.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de <see cref="AlunoListItemDto"/> ordenada por nome.</returns>
    public async Task<List<AlunoListItemDto>> Handle(GetAlunosQuery request, CancellationToken ct)
    {
        var list = await _db.Alunos
            .OrderBy(a => a.Nome)
            .Select(a => new AlunoListItemDto
            {
                Id = a.Id,
                Nome = a.Nome,
                Email = a.Email,
                Cpf = a.Cpf,
                Idade = a.Idade
            })
            .ToListAsync(ct);

        return list;
    }
}