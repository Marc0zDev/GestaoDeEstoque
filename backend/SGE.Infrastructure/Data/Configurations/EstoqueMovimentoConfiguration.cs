using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGE.Domain.Entities;

namespace SGE.Infrastructure.Data.Configurations;

public class EstoqueMovimentoConfiguration : IEntityTypeConfiguration<EstoqueMovimento>
{
    public void Configure(EntityTypeBuilder<EstoqueMovimento> builder)
    {
        builder.HasKey(em => em.Id);

        builder.Property(em => em.Quantidade)
            .IsRequired()
            .HasColumnType("decimal(18,3)");

        builder.Property(em => em.QuantidadeAnterior)
            .IsRequired()
            .HasColumnType("decimal(18,3)");

        builder.Property(em => em.QuantidadeAtual)
            .IsRequired()
            .HasColumnType("decimal(18,3)");

        builder.Property(em => em.Observacoes)
            .HasMaxLength(500);

        builder.Property(em => em.DataMovimento)
            .IsRequired();

        // Configuração do enum TipoMovimento
        builder.Property(em => em.TipoMovimento)
            .HasConversion<string>()
            .IsRequired();

        // Relationships
        builder.HasOne(em => em.EstoqueItem)
            .WithMany(e => e.Movimentos)
            .HasForeignKey(em => em.EstoqueItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}