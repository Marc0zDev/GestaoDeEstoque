using Microsoft.EntityFrameworkCore;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Data.Repositories;

public class LocalArmazenagemRepository : ILocalArmazenagemRepository
{
    private readonly ApplicationDbContext _context;

    public LocalArmazenagemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LocalArmazenagem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LocaisArmazenagem
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<List<LocalArmazenagem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LocaisArmazenagem
            .OrderBy(l => l.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LocalArmazenagem>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LocaisArmazenagem
            .Where(l => l.Ativo)
            .OrderBy(l => l.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LocalArmazenagem>> SearchAsync(string termo, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();
        return await _context.LocaisArmazenagem
            .Where(l => l.Nome.ToLower().Contains(termoLower) ||
                       (l.Descricao != null && l.Descricao.ToLower().Contains(termoLower)))
            .OrderBy(l => l.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LocalArmazenagem local, CancellationToken cancellationToken = default)
    {
        await _context.LocaisArmazenagem.AddAsync(local, cancellationToken);
    }

    public async Task UpdateAsync(LocalArmazenagem local, CancellationToken cancellationToken = default)
    {
        _context.LocaisArmazenagem.Update(local);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var local = await GetByIdAsync(id, cancellationToken);
        if (local != null)
        {
            _context.LocaisArmazenagem.Remove(local);
        }
    }

    public async Task<bool> ExistsWithNomeAsync(string nome, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.LocaisArmazenagem.Where(l => l.Nome == nome);
        
        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasEstoqueItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EstoqueItens
            .AnyAsync(e => e.LocalArmazenagemId == id, cancellationToken);
    }
}