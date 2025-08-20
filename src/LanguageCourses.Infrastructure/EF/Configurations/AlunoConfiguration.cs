using LanguageCourses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCourses.Infrastructure.EF.Configurations;

public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder.ToTable("Alunos");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome).HasMaxLength(150).IsRequired();
        builder.Property(a => a.Email).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Cpf).HasMaxLength(11).IsRequired();
        builder.Property(a => a.Idade).IsRequired();

        builder.HasIndex(a => a.Email).IsUnique();
        builder.HasIndex(a => a.Cpf).IsUnique();
    }
}