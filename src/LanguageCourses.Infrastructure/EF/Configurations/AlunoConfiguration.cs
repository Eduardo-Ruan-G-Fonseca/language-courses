using LanguageCourses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCourses.Infrastructure.EF.Configurations;

/// <summary>
/// Configuração da entidade <see cref="Aluno"/> para o Entity Framework Core.
/// </summary>
/// <remarks>
/// Define o mapeamento da entidade para a tabela <c>Alunos</c>, incluindo
/// restrições de chaves, índices e tamanhos máximos das colunas.
/// 
/// <para><strong>Regras aplicadas na configuração:</strong></para>
/// <list type="bullet">
/// <item><description>Nome obrigatório, com no máximo 150 caracteres.</description></item>
/// <item><description>Email obrigatório, com no máximo 200 caracteres e índice único.</description></item>
/// <item><description>CPF obrigatório, com 11 caracteres e índice único.</description></item>
/// <item><description>Idade obrigatória.</description></item>
/// </list>
/// </remarks>
public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    /// <summary>
    /// Aplica as configurações da entidade <see cref="Aluno"/> no modelo de dados.
    /// </summary>
    /// <param name="builder">Construtor de configuração para a entidade.</param>
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        // Nome da tabela
        builder.ToTable("Alunos");

        // Chave primária
        builder.HasKey(a => a.Id);

        // Propriedades obrigatórias e tamanhos máximos
        builder.Property(a => a.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(a => a.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Cpf)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(a => a.Idade)
            .IsRequired();

        // Índices únicos
        builder.HasIndex(a => a.Email).IsUnique();
        builder.HasIndex(a => a.Cpf).IsUnique();
    }
}