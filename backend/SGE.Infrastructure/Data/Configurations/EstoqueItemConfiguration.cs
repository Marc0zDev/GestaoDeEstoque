using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGE.Domain.Entities;

namespace SGE.Infrastructure.Data.Configurations;

public class EstoqueItemConfiguration : IEntityTypeConfiguration<EstoqueItem>
{
    public void Configure(EntityTypeBuilder<EstoqueItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Quantidade)
            .IsRequired()
            .HasColumnType("decimal(18,3)");

        builder.Property(e => e.PrecoUltimaCompra)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.DataUltimaMovimentacao);

        // Relationships
        builder.HasOne(e => e.Produto)
            .WithMany(p => p.EstoqueItens)
            .HasForeignKey(e => e.ProdutoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.LocalArmazenagem)
            .WithMany(l => l.EstoqueItens)
            .HasForeignKey(e => e.LocalArmazenagemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint para produto + local
        builder.HasIndex(e => new { e.ProdutoId, e.LocalArmazenagemId })
            .IsUnique();
    }
}