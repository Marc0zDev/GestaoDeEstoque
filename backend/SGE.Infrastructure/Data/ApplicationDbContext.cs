using Microsoft.EntityFrameworkCore;
using SGE.Domain.Entities;
using SGE.Domain.Common;
using SGE.Domain.ValueObjects;
using SGE.Infrastructure.Data.Configurations;
using System.Reflection;

namespace SGE.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<LocalArmazenagem> LocaisArmazenagem => Set<LocalArmazenagem>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<EstoqueItem> EstoqueItens => Set<EstoqueItem>();
    public DbSet<EstoqueMovimento> EstoqueMovimentos => Set<EstoqueMovimento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configuração de Value Objects
        modelBuilder.Entity<Usuario>()
            .OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(255)
                    .IsRequired();

                email.HasIndex(e => e.Value)
                    .IsUnique();
            });

        modelBuilder.Entity<Fornecedor>()
            .OwnsOne(f => f.Cnpj, cnpj =>
            {
                cnpj.Property(c => c.Value)
                    .HasColumnName("Cnpj")
                    .HasMaxLength(14)
                    .IsRequired();

                cnpj.HasIndex(c => c.Value)
                    .IsUnique();
            });

        // Configuração de conversores para enums
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<EstoqueMovimento>()
            .Property(em => em.TipoMovimento)
            .HasConversion<string>();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.GetType().GetProperty("CriadoEm")?.SetValue(entry.Entity, DateTime.UtcNow);
                    break;
                case EntityState.Modified:
                    entry.Entity.MarcarComoAtualizado();
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}