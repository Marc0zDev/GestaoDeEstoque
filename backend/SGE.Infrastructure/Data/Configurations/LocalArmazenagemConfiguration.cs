using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGE.Domain.Entities;

namespace SGE.Infrastructure.Data.Configurations;

public class LocalArmazenagemConfiguration : IEntityTypeConfiguration<LocalArmazenagem>
{
    public void Configure(EntityTypeBuilder<LocalArmazenagem> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Descricao)
            .HasMaxLength(500);

        builder.Property(l => l.Ativo)
            .IsRequired();

        builder.HasIndex(l => l.Nome)
            .IsUnique();
    }
}