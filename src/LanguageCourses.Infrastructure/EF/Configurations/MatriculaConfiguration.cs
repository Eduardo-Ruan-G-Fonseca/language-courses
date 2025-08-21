using LanguageCourses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCourses.Infrastructure.EF.Configurations;

/// <summary>
/// Configuração da entidade <see cref="Matricula"/> para o Entity Framework Core.
/// </summary>
/// <remarks>
/// Define o mapeamento da entidade para a tabela <c>Matriculas</c>, estabelecendo
/// a chave composta, relacionamentos e restrições de exclusão.
/// 
/// <para><strong>Regras aplicadas na configuração:</strong></para>
/// <list type="bullet">
/// <item><description>Chave primária composta por <see cref="Matricula.AlunoId"/> e <see cref="Matricula.TurmaId"/>.</description></item>
/// <item><description><see cref="Matricula.DataMatricula"/> obrigatório, armazenado como <c>datetime2</c>.</description></item>
/// <item><description>Um aluno pode ter várias matrículas, mas não pode ser excluído se possuir alguma (DeleteBehavior.Restrict).</description></item>
/// <item><description>Uma turma pode ter várias matrículas, mas não pode ser excluída se possuir alunos (DeleteBehavior.Restrict).</description></item>
/// </list>
/// </remarks>
public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
{
    /// <summary>
    /// Aplica as configurações da entidade <see cref="Matricula"/> no modelo de dados.
    /// </summary>
    /// <param name="builder">Construtor de configuração para a entidade.</param>
    public void Configure(EntityTypeBuilder<Matricula> builder)
    {
        // Nome da tabela
        builder.ToTable("Matriculas");

        // Chave primária composta
        builder.HasKey(m => new { m.AlunoId, m.TurmaId });

        // Propriedade obrigatória com tipo explícito
        builder.Property(m => m.DataMatricula)
               .HasColumnType("datetime2")
               .IsRequired();

        // Relacionamento: Aluno 1..N Matrículas
        builder.HasOne(m => m.Aluno)
               .WithMany(a => a.Matriculas)
               .HasForeignKey(m => m.AlunoId)
               .OnDelete(DeleteBehavior.Restrict); // impede excluir aluno com matrícula

        // Relacionamento: Turma 1..N Matrículas
        builder.HasOne(m => m.Turma)
               .WithMany(t => t.Matriculas)
               .HasForeignKey(m => m.TurmaId)
               .OnDelete(DeleteBehavior.Restrict); // impede excluir turma com alunos
    }
}
