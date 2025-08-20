using LanguageCourses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCourses.Infrastructure.EF.Configurations;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        builder.ToTable("Turmas");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Numero).IsRequired();
        builder.Property(t => t.Idioma).HasMaxLength(50).IsRequired();
        builder.Property(t => t.AnoLetivo).HasMaxLength(16).IsRequired();

        // Matrícula por (Numero, Idioma)
        builder.HasIndex(t => new { t.Numero, t.Idioma }).IsUnique();
    }
}