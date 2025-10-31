using Microsoft.EntityFrameworkCore;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Data.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _context;

    public ProdutoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Produto?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .FirstOrDefaultAsync(p => p.Codigo == codigo, cancellationToken);
    }

    public async Task<List<Produto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Produto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Produto>> GetByCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .Where(p => p.CategoriaId == categoriaId)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Produto>> GetByFornecedorAsync(Guid fornecedorId, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .Where(p => p.FornecedorId == fornecedorId)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Produto>> SearchAsync(string termo, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .Where(p => p.Nome.ToLower().Contains(termoLower) || 
                       p.Codigo.ToLower().Contains(termoLower) ||
                       (p.Descricao != null && p.Descricao.ToLower().Contains(termoLower)))
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Produto>> GetLowStockAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .Include(p => p.EstoqueItens)
            .Where(p => p.Ativo && p.EstoqueItens.Sum(e => e.Quantidade) <= p.EstoqueMinimo)
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        await _context.Produtos.AddAsync(produto, cancellationToken);
    }

    public async Task UpdateAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        _context.Produtos.Update(produto);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var produto = await GetByIdAsync(id, cancellationToken);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
        }
    }

    public async Task<bool> ExistsWithCodigoAsync(string codigo, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Produtos.Where(p => p.Codigo == codigo);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasEstoqueItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .AnyAsync(e => e.ProdutoId == id, cancellationToken);
    }
}