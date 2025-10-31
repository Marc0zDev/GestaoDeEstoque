using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGE.Domain.Entities;

namespace SGE.Infrastructure.Data.Configurations;

public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Telefone)
            .HasMaxLength(20);

        builder.Property(f => f.Email)
            .HasMaxLength(255);

        builder.Property(f => f.Endereco)
            .HasMaxLength(500);

        builder.Property(f => f.Ativo)
            .IsRequired();

        builder.HasIndex(f => f.Cnpj.Value)
            .IsUnique();
    }
}