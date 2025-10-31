using SGE.Domain.Entities;

namespace SGE.Domain.Interfaces;

public interface ILocalArmazenagemRepository
{
    Task<LocalArmazenagem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<LocalArmazenagem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<LocalArmazenagem>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<LocalArmazenagem>> SearchAsync(string termo, CancellationToken cancellationToken = default);
    Task AddAsync(LocalArmazenagem local, CancellationToken cancellationToken = default);
    Task UpdateAsync(LocalArmazenagem local, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNomeAsync(string nome, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasEstoqueItemsAsync(Guid id, CancellationToken cancellationToken = default);
}