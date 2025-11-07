using Microsoft.EntityFrameworkCore;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Data.Repositories;

public class FornecedorRepository : IFornecedorRepository
{
    private readonly ApplicationDbContext _context;

    public FornecedorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Fornecedor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<Fornecedor?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .FirstOrDefaultAsync(f => f.Cnpj.Value == cnpj, cancellationToken);
    }

    public async Task<List<Fornecedor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .OrderBy(f => f.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Fornecedor>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .Where(f => f.Ativo)
            .OrderBy(f => f.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Fornecedor>> SearchAsync(string termo, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();
        return await _context.Fornecedores
            .Where(f => f.Nome.ToLower().Contains(termoLower) || 
                       f.Cnpj.Value.Contains(termo) ||
                       (f.Email != null && f.Email.ToLower().Contains(termoLower)))
            .OrderBy(f => f.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Fornecedor fornecedor, CancellationToken cancellationToken = default)
    {
        await _context.Fornecedores.AddAsync(fornecedor, cancellationToken);
    }

    public async Task UpdateAsync(Fornecedor fornecedor, CancellationToken cancellationToken = default)
    {
        _context.Fornecedores.Update(fornecedor);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fornecedor = await GetByIdAsync(id, cancellationToken);
        if (fornecedor != null)
        {
            _context.Fornecedores.Remove(fornecedor);
        }
    }

    public async Task<bool> ExistsWithCnpjAsync(string cnpj, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Fornecedores.Where(f => f.Cnpj.Value == cnpj);
        
        if (excludeId.HasValue)
            query = query.Where(f => f.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .AnyAsync(p => p.FornecedorId == id, cancellationToken);
    }

    public async Task<List<Fornecedor>> GetAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Fornecedores
            .Where(f => f.Ativo)
            .OrderBy(f => f.Nome)
            .ToListAsync(cancellationToken);
    }

    public void Update(Fornecedor fornecedor)
    {
        _context.Fornecedores.Update(fornecedor);
    }

    public void Delete(Fornecedor fornecedor)
    {
        _context.Fornecedores.Remove(fornecedor);
    }
}