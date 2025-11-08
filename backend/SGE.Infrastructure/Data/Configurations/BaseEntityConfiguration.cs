using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGE.Domain.Common;

namespace SGE.Infrastructure.Data.Configurations;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // Configuração da BaseEntity
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .ValueGeneratedNever();
            
        builder.Property(e => e.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(e => e.CriadoEm)
            .IsRequired();
            
        builder.Property(e => e.AtualizadoEm);

        // Permitir que classes derivadas adicionem configurações específicas
        ConfigureEntity(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
}