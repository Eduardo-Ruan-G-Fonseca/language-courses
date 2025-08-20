using LanguageCourses.Application.ViewModels;
using LanguageCourses.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Application.Commands.Alunos;

public class DesmatricularAlunoHandler : IRequestHandler<DesmatricularAlunoCommand, AlunoDetalheDto>
{
    private readonly LanguageCoursesDbContext _db;
    public DesmatricularAlunoHandler(LanguageCoursesDbContext db) => _db = db;

    public async Task<AlunoDetalheDto> Handle(DesmatricularAlunoCommand request, CancellationToken ct)
    {
        var aluno = await _db.Alunos
            .Include(a => a.Matriculas).ThenInclude(m => m.Turma)
            .FirstOrDefaultAsync(a => a.Id == request.AlunoId, ct);

        if (aluno is null)
            throw new KeyNotFoundException("Aluno não encontrado.");

        var idioma = request.Matricula.Idioma.Trim();
        var numero = request.Matricula.Numero;

        var turma = await _db.Turmas
            .FirstOrDefaultAsync(t => t.Numero == numero && t.Idioma.ToLower() == idioma.ToLower(), ct);

        if (turma is null)
            throw new KeyNotFoundException($"Turma {idioma} {numero} não encontrada.");

        var mat = aluno.Matriculas.FirstOrDefault(m => m.TurmaId == turma.Id);
        if (mat is null)
            throw new InvalidOperationException("Aluno não está matriculado nessa turma.");

        // regra: após desmatricular, aluno deve continuar com ≥ 1 turma
        if (aluno.Matriculas.Count <= 1)
            throw new InvalidOperationException("Aluno deve permanecer com pelo menos 1 turma.");

        _db.Matriculas.Remove(mat);
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
