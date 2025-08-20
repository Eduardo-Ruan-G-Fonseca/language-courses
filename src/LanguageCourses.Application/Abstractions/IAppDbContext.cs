using LanguageCourses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageCourses.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Aluno> Alunos { get; }
    DbSet<Turma> Turmas { get; }
    DbSet<Matricula> Matriculas { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}