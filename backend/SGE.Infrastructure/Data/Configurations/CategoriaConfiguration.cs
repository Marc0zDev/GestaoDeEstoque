using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGE.Domain.Entities;

namespace SGE.Infrastructure.Data.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Descricao)
            .HasMaxLength(500);

        // Self-referencing relationship
        builder.HasOne(c => c.CategoriaParent)
            .WithMany(c => c.SubCategorias)
            .HasForeignKey(c => c.CategoriaParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint para nome por categoria pai
        builder.HasIndex(c => new { c.Nome, c.CategoriaParentId })
            .IsUnique();
    }
}