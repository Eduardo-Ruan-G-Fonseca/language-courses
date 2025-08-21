using LanguageCourses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCourses.Infrastructure.EF.Configurations;

/// <summary>
/// Configuração da entidade <see cref="Turma"/> para o Entity Framework Core.
/// </summary>
/// <remarks>
/// Define o mapeamento da entidade para a tabela <c>Turmas</c>, incluindo chaves,
/// restrições, índices e propriedades obrigatórias.
/// 
/// <para><strong>Regras aplicadas na configuração:</strong></para>
/// <list type="bullet">
/// <item><description><see cref="Turma.Numero"/> é obrigatório.</description></item>
/// <item><description><see cref="Turma.Idioma"/> é obrigatório, com tamanho máximo de 50 caracteres.</description></item>
/// <item><description><see cref="Turma.AnoLetivo"/> é obrigatório, com tamanho máximo de 16 caracteres (ex.: "2025/1").</description></item>
/// <item><description>Índice único definido pela combinação de <see cref="Turma.Numero"/> e <see cref="Turma.Idioma"/> para evitar duplicidade de turmas no mesmo idioma e número.</description></item>
/// </list>
/// </remarks>
public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
    /// <summary>
    /// Aplica as configurações da entidade <see cref="Turma"/> no modelo de dados.
    /// </summary>
    /// <param name="builder">Construtor de configuração para a entidade.</param>
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        // Nome da tabela
        builder.ToTable("Turmas");

        // Chave primária
        builder.HasKey(t => t.Id);

        // Propriedades obrigatórias
        builder.Property(t => t.Numero)
               .IsRequired();

        builder.Property(t => t.Idioma)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(t => t.AnoLetivo)
               .HasMaxLength(16)
               .IsRequired();

        // Índice único para garantir que não existam turmas duplicadas
        builder.HasIndex(t => new { t.Numero, t.Idioma })
               .IsUnique();
    }
}
