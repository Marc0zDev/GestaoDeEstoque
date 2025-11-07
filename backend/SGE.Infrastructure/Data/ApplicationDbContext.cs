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
        // Aplica todas as configurações dos arquivos IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

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