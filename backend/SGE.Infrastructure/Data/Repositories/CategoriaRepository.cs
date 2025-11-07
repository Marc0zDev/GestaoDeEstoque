using Microsoft.EntityFrameworkCore;
using SGE.Domain.Entities;
using SGE.Domain.Interfaces;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Data.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ApplicationDbContext _context;

    public CategoriaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Include(c => c.CategoriaParent)
            .Include(c => c.SubCategorias)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Categoria>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Include(c => c.CategoriaParent)
            .Include(c => c.SubCategorias)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Categoria>> GetByParentIdAsync(Guid? parentId, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Include(c => c.CategoriaParent)
            .Include(c => c.SubCategorias)
            .Where(c => c.CategoriaParentId == parentId)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Categoria>> GetHierarchyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var hierarchy = new List<Categoria>();
        var categoria = await GetByIdAsync(id, cancellationToken);
        
        while (categoria != null)
        {
            hierarchy.Insert(0, categoria);
            if (categoria.CategoriaParentId.HasValue)
            {
                categoria = await GetByIdAsync(categoria.CategoriaParentId.Value, cancellationToken);
            }
            else
            {
                break;
            }
        }

        return hierarchy;
    }

    public async Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        await _context.Categorias.AddAsync(categoria, cancellationToken);
    }

    public async Task UpdateAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        _context.Categorias.Update(categoria);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var categoria = await GetByIdAsync(id, cancellationToken);
        if (categoria != null)
        {
            _context.Categorias.Remove(categoria);
        }
    }

    public async Task<bool> ExistsWithNameAsync(string nome, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Categorias.Where(c => c.Nome == nome);
        
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .AnyAsync(c => c.CategoriaParentId == id, cancellationToken);
    }

    public async Task<bool> HasProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .AnyAsync(p => p.CategoriaId == id, cancellationToken);
    }

    public async Task<List<Categoria>> GetAtivasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .Include(c => c.CategoriaParent)
            .Include(c => c.SubCategorias)
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Categoria?> GetByNameAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Nome == nome, cancellationToken);
    }

    public void Update(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
    }

    public void Delete(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
    }
}