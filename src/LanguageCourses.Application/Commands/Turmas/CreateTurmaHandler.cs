using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using LanguageCourses.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Turmas;

/// <summary>
/// Handler responsável por processar o comando <see cref="CreateTurmaCommand"/>.
/// </summary>
/// <remarks>
/// Este handler realiza a criação de uma nova turma no sistema,
/// garantindo que não existam duplicidades para a combinação de
/// Idioma e Número. Caso uma turma já exista com os mesmos valores,
/// uma exceção é lançada.
/// 
/// Fluxo de execução:
/// 1. Recebe um <see cref="CreateTurmaCommand"/> com os dados da nova turma.
/// 2. Verifica se já existe uma turma com o mesmo Idioma e Número.
/// 3. Caso não exista, cria uma nova instância de <see cref="Turma"/> e persiste no banco.
/// 4. Retorna um objeto <see cref="TurmaDto"/> com os dados da turma criada.
/// </remarks>
public class CreateTurmaHandler : IRequestHandler<CreateTurmaCommand, TurmaDto>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="CreateTurmaHandler"/>.
    /// </summary>
    /// <param name="db">Contexto de acesso ao banco de dados da aplicação.</param>
    public CreateTurmaHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Processa o comando de criação de turma.
    /// </summary>
    /// <param name="request">
    /// Objeto <see cref="CreateTurmaCommand"/> contendo os dados da nova turma.
    /// </param>
    /// <param name="ct">Token de cancelamento da operação assíncrona.</param>
    /// <returns>
    /// Um objeto <see cref="TurmaDto"/> representando a turma criada.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando já existe uma turma com o mesmo Idioma e Número.
    /// </exception>
    public async Task<TurmaDto> Handle(CreateTurmaCommand request, CancellationToken ct)
    {
        var idioma = request.Turma.Idioma.Trim();
        var numero = request.Turma.Numero;
        
        var exists = await _db.Turmas.AnyAsync(t =>
            t.Numero == numero &&
            t.Idioma.ToLower() == idioma.ToLower(), ct);

        if (exists)
            throw new InvalidOperationException($"Já existe uma turma {idioma} {numero}.");

        var turma = new Turma
        {
            Numero = numero,
            Idioma = idioma,
            AnoLetivo = request.Turma.AnoLetivo.Trim()
        };

        _db.Turmas.Add(turma);
        await _db.SaveChangesAsync(ct);

        return new TurmaDto
        {
            Id = turma.Id,
            Numero = turma.Numero,
            Idioma = turma.Idioma,
            AnoLetivo = turma.AnoLetivo,
            VagasRestantes = 5
        };
    }
}
