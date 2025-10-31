using SGE.Domain.Entities;

namespace SGE.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Produto?> GetByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<List<Produto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Produto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<Produto>> GetByCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken = default);
    Task<List<Produto>> GetByFornecedorAsync(Guid fornecedorId, CancellationToken cancellationToken = default);
    Task<List<Produto>> SearchAsync(string termo, CancellationToken cancellationToken = default);
    Task<List<Produto>> GetLowStockAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Produto produto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Produto produto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithCodigoAsync(string codigo, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasEstoqueItemsAsync(Guid id, CancellationToken cancellationToken = default);
}