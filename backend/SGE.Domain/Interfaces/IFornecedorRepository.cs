using SGE.Domain.Entities;

namespace SGE.Domain.Interfaces;

public interface IFornecedorRepository
{
    Task<Fornecedor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Fornecedor?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<List<Fornecedor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Fornecedor>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<Fornecedor>> GetAtivosAsync(CancellationToken cancellationToken = default);
    Task<List<Fornecedor>> SearchAsync(string termo, CancellationToken cancellationToken = default);
    Task AddAsync(Fornecedor fornecedor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Fornecedor fornecedor, CancellationToken cancellationToken = default);
    void Update(Fornecedor fornecedor);
    void Delete(Fornecedor fornecedor);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithCnpjAsync(string cnpj, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(Guid id, CancellationToken cancellationToken = default);
}