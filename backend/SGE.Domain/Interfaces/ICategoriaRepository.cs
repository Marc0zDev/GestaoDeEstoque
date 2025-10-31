using SGE.Domain.Entities;

namespace SGE.Domain.Interfaces;

public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Categoria>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Categoria>> GetByParentIdAsync(Guid? parentId, CancellationToken cancellationToken = default);
    Task<List<Categoria>> GetHierarchyAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default);
    Task UpdateAsync(Categoria categoria, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameAsync(string nome, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(Guid id, CancellationToken cancellationToken = default);
}