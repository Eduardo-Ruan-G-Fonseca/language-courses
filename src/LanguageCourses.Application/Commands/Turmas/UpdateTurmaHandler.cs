using LanguageCourses.Infrastructure.EF;
using LanguageCourses.Application.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Turmas;

/// <summary>
/// Manipulador responsável por processar o comando <see cref="UpdateTurmaCommand"/>.
/// Atualiza os dados de uma turma existente no sistema.
/// </summary>
public class UpdateTurmaHandler : IRequestHandler<UpdateTurmaCommand, TurmaDto>
{
    private readonly LanguageCoursesDbContext _db;

    /// <summary>
    /// Construtor que injeta o contexto do banco de dados.
    /// </summary>
    /// <param name="db">Instância do contexto <see cref="LanguageCoursesDbContext"/>.</param>
    public UpdateTurmaHandler(LanguageCoursesDbContext db) => _db = db;

    /// <summary>
    /// Processa o comando para atualização de uma turma.
    /// </summary>
    /// <param name="request">Comando contendo o identificador da turma e os novos dados.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Um <see cref="TurmaDto"/> com os dados atualizados da turma.</returns>
    /// <exception cref="KeyNotFoundException">Lançada caso a turma não seja encontrada.</exception>
    /// <exception cref="InvalidOperationException">Lançada caso já exista outra turma com o mesmo idioma e número.</exception>
    public async Task<TurmaDto> Handle(UpdateTurmaCommand request, CancellationToken ct)
    {
        var turma = await _db.Turmas.Include(t => t.Matriculas)
            .FirstOrDefaultAsync(t => t.Id == request.Id, ct);

        if (turma is null)
            throw new KeyNotFoundException("Turma não encontrada.");

        var novoIdioma = request.Turma.Idioma.Trim();
        var novoNumero = request.Turma.Numero;
        
        var conflito = await _db.Turmas.AnyAsync(t =>
            t.Id != turma.Id &&
            t.Numero == novoNumero &&
            t.Idioma.ToLower() == novoIdioma.ToLower(), ct);

        if (conflito)
            throw new InvalidOperationException($"Já existe a turma {novoIdioma} {novoNumero}.");

        turma.Numero = novoNumero;
        turma.Idioma = novoIdioma;
        turma.AnoLetivo = request.Turma.AnoLetivo.Trim();

        await _db.SaveChangesAsync(ct);

        return new TurmaDto
        {
            Id = turma.Id,
            Numero = turma.Numero,
            Idioma = turma.Idioma,
            AnoLetivo = turma.AnoLetivo,
            VagasRestantes = 5 - turma.Matriculas.Count
        };
    }
}
