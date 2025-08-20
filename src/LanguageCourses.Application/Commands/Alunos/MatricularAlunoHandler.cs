using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Alunos;

public class MatricularAlunoHandler : IRequestHandler<MatricularAlunoCommand, AlunoDetalheDto>
{
    private readonly LanguageCoursesDbContext _db;
    public MatricularAlunoHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<AlunoDetalheDto> Handle(MatricularAlunoCommand request, CancellationToken ct)
    {
        var aluno = await _db.Alunos
            .Include(a => a.Matriculas).ThenInclude(m => m.Turma)
            .FirstOrDefaultAsync(a => a.Id == request.AlunoId, ct);

        if (aluno is null)
            throw new KeyNotFoundException("Aluno não encontrado.");

        var idioma = request.Matricula.Idioma.Trim();
        var numero = request.Matricula.Numero;

        var turma = await _db.Turmas
            .Include(t => t.Matriculas)
            .FirstOrDefaultAsync(t => t.Numero == numero && t.Idioma.ToLower() == idioma.ToLower(), ct);

        if (turma is null)
            throw new KeyNotFoundException($"Turma {idioma} {numero} não encontrada.");

        if (aluno.Matriculas.Any(m => m.TurmaId == turma.Id))
            throw new InvalidOperationException("Aluno já está matriculado nessa turma.");

        if (turma.Matriculas.Count >= 5)
            throw new InvalidOperationException($"Turma {turma.Idioma} {turma.Numero} está lotada (máx. 5).");

        _db.Matriculas.Add(new LanguageCourses.Domain.Entities.Matricula
        {
            AlunoId = aluno.Id,
            TurmaId = turma.Id,
            DataMatricula = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);

        // retorno
        await _db.Entry(aluno).Collection(a => a.Matriculas).Query().Include(m => m.Turma).LoadAsync(ct);
        return new AlunoDetalheDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            Cpf = aluno.Cpf,
            Idade = aluno.Idade,
            Turmas = aluno.Matriculas.Select(m => new TurmaResumoDto
            {
                Id = m.Turma.Id, Idioma = m.Turma.Idioma, Numero = m.Turma.Numero, AnoLetivo = m.Turma.AnoLetivo
            }).OrderBy(t => t.Idioma).ThenBy(t => t.Numero).ToList()
        };
    }
}
