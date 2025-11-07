using SGE.Domain.Entities;

namespace SGE.Domain.Interfaces;

public interface IEstoqueRepository
{
    // EstoqueItem operations
    Task<EstoqueItem?> GetEstoqueItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EstoqueItem?> GetEstoqueItemAsync(Guid produtoId, Guid localId, CancellationToken cancellationToken = default);
    Task<List<EstoqueItem>> GetEstoqueByProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default);
    Task<List<EstoqueItem>> GetEstoqueByLocalAsync(Guid localId, CancellationToken cancellationToken = default);
    Task<List<EstoqueItem>> GetAllEstoqueItemsAsync(CancellationToken cancellationToken = default);
    Task<List<EstoqueItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task AddEstoqueItemAsync(EstoqueItem estoqueItem, CancellationToken cancellationToken = default);
    Task UpdateEstoqueItemAsync(EstoqueItem estoqueItem, CancellationToken cancellationToken = default);
    Task DeleteEstoqueItemAsync(Guid id, CancellationToken cancellationToken = default);

    // EstoqueMovimento operations
    Task<EstoqueMovimento?> GetMovimentoByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<EstoqueMovimento>> GetMovimentosByEstoqueItemAsync(Guid estoqueItemId, CancellationToken cancellationToken = default);
    Task<List<EstoqueMovimento>> GetMovimentosByProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default);
    Task<List<EstoqueMovimento>> GetMovimentosByPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
    Task<List<EstoqueMovimento>> GetMovimentosByTipoAsync(Enums.TipoMovimento tipo, CancellationToken cancellationToken = default);
    Task<List<EstoqueMovimento>> GetAllMovimentosAsync(CancellationToken cancellationToken = default);
    Task AddMovimentoAsync(EstoqueMovimento movimento, CancellationToken cancellationToken = default);
    Task UpdateMovimentoAsync(EstoqueMovimento movimento, CancellationToken cancellationToken = default);
    Task DeleteMovimentoAsync(Guid id, CancellationToken cancellationToken = default);

    // Business operations
    Task<bool> PodeRemoverQuantidadeAsync(Guid produtoId, Guid localId, decimal quantidade, CancellationToken cancellationToken = default);
    Task<decimal> GetQuantidadeTotalProdutoAsync(Guid produtoId, CancellationToken cancellationToken = default);
    Task<List<EstoqueItem>> GetRelatorioPosicaoEstoqueAsync(CancellationToken cancellationToken = default);
    
    // Relatórios específicos
    Task<List<EstoqueItem>> GetByProdutoIdAsync(Guid produtoId, CancellationToken cancellationToken = default);
    Task<List<EstoqueItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EstoqueMovimento?> GetUltimaMovimentacaoAsync(Guid estoqueItemId, CancellationToken cancellationToken = default);
    Task<List<EstoqueMovimento>> GetMovimentacoesPorPeriodoAsync(
        DateTime dataInicio, 
        DateTime dataFim, 
        Guid? produtoId = null, 
        Enums.TipoMovimento? tipoMovimento = null, 
        Guid? usuarioId = null, 
        Guid? localArmazenagemId = null, 
        CancellationToken cancellationToken = default);
}