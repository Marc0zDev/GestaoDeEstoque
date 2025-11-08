using Microsoft.EntityFrameworkCore;
using SGE.Domain.Entities;
using SGE.Domain.Enums;
using SGE.Domain.Interfaces;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Data.Repositories;

public class EstoqueRepository : IEstoqueRepository
{
    private readonly ApplicationDbContext _context;

    public EstoqueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region EstoqueItem operations

    public async Task<EstoqueItem?> GetEstoqueItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
            .Include(e => e.LocalArmazenagem)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<EstoqueItem?> GetEstoqueItemAsync(Guid produtoId, Guid localId, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
            .Include(e => e.LocalArmazenagem)
            .FirstOrDefaultAsync(e => e.ProdutoId == produtoId && e.LocalArmazenagemId == localId, cancellationToken);
    }

    public async Task<List<EstoqueItem>> GetEstoqueByProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
            .Include(e => e.LocalArmazenagem)
            .Where(e => e.ProdutoId == produtoId)
            .OrderBy(e => e.LocalArmazenagem.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EstoqueItem>> GetEstoqueByLocalAsync(Guid localId, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
            .Include(e => e.LocalArmazenagem)
            .Where(e => e.LocalArmazenagemId == localId)
            .OrderBy(e => e.Produto.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EstoqueItem>> GetAllEstoqueItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
            .Include(e => e.LocalArmazenagem)
            .OrderBy(e => e.Produto.Nome)
            .ThenBy(e => e.LocalArmazenagem.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EstoqueItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
            .Include(e => e.LocalArmazenagem)
            .Where(e => e.Produto != null && e.Produto.Ativo)
            .GroupBy(e => e.ProdutoId)
            .Select(g => new { ProdutoId = g.Key, TotalQuantidade = g.Sum(e => e.Quantidade) })
            .Join(_context.Produtos, 
                  estoque => estoque.ProdutoId, 
                  produto => produto.Id, 
                  (estoque, produto) => new { estoque.TotalQuantidade, produto })
            .Where(x => x.TotalQuantidade <= x.produto.EstoqueMinimo && x.produto.Ativo)
            .SelectMany(x => _context.EstoqueItens
                .Include(e => e.Produto)
                .Include(e => e.LocalArmazenagem)
                .Where(e => e.ProdutoId == x.produto.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddEstoqueItemAsync(EstoqueItem estoqueItem, CancellationToken cancellationToken = default)
    {
        await _context.EstoqueItens.AddAsync(estoqueItem, cancellationToken);
    }

    public async Task UpdateEstoqueItemAsync(EstoqueItem estoqueItem, CancellationToken cancellationToken = default)
    {
        _context.EstoqueItens.Update(estoqueItem);
        await Task.CompletedTask;
    }

    public async Task DeleteEstoqueItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await GetEstoqueItemByIdAsync(id, cancellationToken);
        if (item != null)
        {
            _context.EstoqueItens.Remove(item);
        }
    }

    #endregion

    #region EstoqueMovimento operations

    public async Task<EstoqueMovimento?> GetMovimentoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueMovimentos
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.LocalArmazenagem)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<List<EstoqueMovimento>> GetMovimentosByEstoqueItemAsync(Guid estoqueItemId, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueMovimentos
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.LocalArmazenagem)
            .Where(m => m.EstoqueItemId == estoqueItemId)
            .OrderByDescending(m => m.DataMovimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EstoqueMovimento>> GetMovimentosByProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueMovimentos
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.LocalArmazenagem)
            .Where(m => m.EstoqueItem.ProdutoId == produtoId)
            .OrderByDescending(m => m.DataMovimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EstoqueMovimento>> GetMovimentosByPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueMovimentos
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.LocalArmazenagem)
            .Where(m => m.DataMovimento >= dataInicio && m.DataMovimento <= dataFim)
            .OrderByDescending(m => m.DataMovimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EstoqueMovimento>> GetMovimentosByTipoAsync(TipoMovimento tipo, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueMovimentos
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.LocalArmazenagem)
            .Where(m => m.TipoMovimento == tipo)
            .OrderByDescending(m => m.DataMovimento)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EstoqueMovimento>> GetAllMovimentosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueMovimentos
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.LocalArmazenagem)
            .OrderByDescending(m => m.DataMovimento)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMovimentoAsync(EstoqueMovimento movimento, CancellationToken cancellationToken = default)
    {
        await _context.EstoqueMovimentos.AddAsync(movimento, cancellationToken);
    }

    public async Task UpdateMovimentoAsync(EstoqueMovimento movimento, CancellationToken cancellationToken = default)
    {
        _context.EstoqueMovimentos.Update(movimento);
        await Task.CompletedTask;
    }

    public async Task DeleteMovimentoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var movimento = await GetMovimentoByIdAsync(id, cancellationToken);
        if (movimento != null)
        {
            _context.EstoqueMovimentos.Remove(movimento);
        }
    }

    #endregion

    #region Business operations

    public async Task<bool> PodeRemoverQuantidadeAsync(Guid produtoId, Guid localId, decimal quantidade, CancellationToken cancellationToken = default)
    {
        var estoqueItem = await GetEstoqueItemAsync(produtoId, localId, cancellationToken);
        return estoqueItem != null && estoqueItem.Quantidade >= quantidade;
    }

    public async Task<decimal> GetQuantidadeTotalProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Where(e => e.ProdutoId == produtoId)
            .SumAsync(e => e.Quantidade, cancellationToken);
    }

    public async Task<List<EstoqueItem>> GetRelatorioPosicaoEstoqueAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
                .ThenInclude(p => p.Categoria)
            .Include(e => e.Produto)
                .ThenInclude(p => p.Fornecedor)
            .Include(e => e.LocalArmazenagem)
            .Where(e => e.Produto != null && e.Produto.Ativo)
            .OrderBy(e => e.Produto.Nome)
            .ThenBy(e => e.LocalArmazenagem.Nome)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Relatórios específicos

    public async Task<List<EstoqueItem>> GetByProdutoIdAsync(Guid produtoId, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
                .ThenInclude(p => p.Categoria)
            .Include(e => e.Produto)
                .ThenInclude(p => p.Fornecedor)
            .Include(e => e.LocalArmazenagem)
            .Where(e => e.ProdutoId == produtoId)
            .OrderBy(e => e.LocalArmazenagem.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EstoqueItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .Include(e => e.Produto)
                .ThenInclude(p => p.Categoria)
            .Include(e => e.Produto)
                .ThenInclude(p => p.Fornecedor)
            .Include(e => e.LocalArmazenagem)
            .Include(e => e.Movimentos)
            .Where(e => e.Ativo && e.Produto != null && e.Produto.Ativo)
            .OrderBy(e => e.Produto.Nome)
            .ThenBy(e => e.LocalArmazenagem.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<EstoqueMovimento?> GetUltimaMovimentacaoAsync(Guid estoqueItemId, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueMovimentos
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.LocalArmazenagem)
            .Where(m => m.EstoqueItemId == estoqueItemId)
            .OrderByDescending(m => m.DataMovimento)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<EstoqueMovimento>> GetMovimentacoesPorPeriodoAsync(
        DateTime dataInicio,
        DateTime dataFim,
        Guid? produtoId = null,
        TipoMovimento? tipoMovimento = null,
        Guid? usuarioId = null,
        Guid? localArmazenagemId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.EstoqueMovimentos
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
                    .ThenInclude(p => p.Categoria)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.Produto)
                    .ThenInclude(p => p.Fornecedor)
            .Include(m => m.EstoqueItem)
                .ThenInclude(e => e.LocalArmazenagem)
            .Where(m => m.DataMovimento >= dataInicio && m.DataMovimento <= dataFim)
            .AsQueryable();

        if (produtoId.HasValue)
        {
            query = query.Where(m => m.EstoqueItem.ProdutoId == produtoId.Value);
        }

        if (tipoMovimento.HasValue)
        {
            query = query.Where(m => m.TipoMovimento == tipoMovimento.Value);
        }

        if (localArmazenagemId.HasValue)
        {
            query = query.Where(m => m.EstoqueItem.LocalArmazenagemId == localArmazenagemId.Value);
        }

        // Note: usuarioId não está implementado na entidade EstoqueMovimento atual
        // Se necessário, seria preciso adicionar essa propriedade à entidade

        return await query
            .OrderByDescending(m => m.DataMovimento)
            .ToListAsync(cancellationToken);
    }

    #endregion
}