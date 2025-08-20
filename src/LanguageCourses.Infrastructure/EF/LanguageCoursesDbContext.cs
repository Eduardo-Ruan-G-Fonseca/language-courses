using LanguageCourses.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LanguageCourses.Infrastructure.EF;

public class LanguageCoursesDbContext : DbContext
{
    public LanguageCoursesDbContext(DbContextOptions<LanguageCoursesDbContext> options) : base(options) { }

    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LanguageCoursesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}