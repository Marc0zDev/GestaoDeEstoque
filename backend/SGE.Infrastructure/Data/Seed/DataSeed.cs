using Microsoft.EntityFrameworkCore;
using SGE.Domain.Entities;
using SGE.Domain.Enums;
using SGE.Domain.ValueObjects;

namespace SGE.Infrastructure.Data.Seed;

public static class DataSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // SEMPRE remove e recria o usuário admin para garantir BCrypt correto
        var existingAdmin = await context.Usuarios.FirstOrDefaultAsync(u => u.Email.Value == "admin@gestaoestoque.com");
        if (existingAdmin != null)
        {
            context.Usuarios.Remove(existingAdmin);
            await context.SaveChangesAsync();
        }

        // Verifica se dados já existem (categorias e fornecedores)
        var categoriaExists = await context.Categorias.AnyAsync();
        var fornecedorExists = await context.Fornecedores.AnyAsync();
        
        if (categoriaExists && fornecedorExists)
        {
            // Só cria o usuário se outros dados já existem
            var newAdminEmail = Email.Create("admin@gestaoestoque.com");
            var newAdminSenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");

            var newAdmin = Usuario.Create(
                "Administrador",
                newAdminEmail.Value,
                newAdminSenhaHash,
                Role.Admin
            );

            await context.Usuarios.AddAsync(newAdmin);
            await context.SaveChangesAsync();
            return;
        }

        // Cria usuário admin padrão usando BCrypt
        var adminEmail = Email.Create("admin@gestaoestoque.com");
        var adminSenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");

        var admin = Usuario.Create(
            "Administrador",
            adminEmail.Value,
            adminSenhaHash,
            Role.Admin
        );

        await context.Usuarios.AddAsync(admin);

        // Cria algumas categorias padrão
        var categorias = new[]
        {
            Categoria.Create("Eletrônicos", "Produtos eletrônicos em geral"),
            Categoria.Create("Informática", "Equipamentos de informática"),
            Categoria.Create("Móveis", "Móveis e decoração"),
            Categoria.Create("Alimentação", "Produtos alimentícios"),
            Categoria.Create("Limpeza", "Produtos de limpeza")
        };

        await context.Categorias.AddRangeAsync(categorias);

        // Cria um fornecedor padrão
        var fornecedor = Fornecedor.Create(
            "Fornecedor Padrão",
            "12345678000195",
            "(11) 99999-9999",
            "contato@fornecedor.com",
            "Rua Exemplo, 123"
        );

        await context.Fornecedores.AddAsync(fornecedor);

        await context.SaveChangesAsync();
    }
}