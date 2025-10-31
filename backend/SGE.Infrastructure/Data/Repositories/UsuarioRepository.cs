using Microsoft.EntityFrameworkCore;
using SGE.Domain.Entities;
using SGE.Domain.Enums;
using SGE.Domain.Interfaces;
using SGE.Infrastructure.Data;

namespace SGE.Infrastructure.Data.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
    }

    public async Task<List<Usuario>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .OrderBy(u => u.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Usuario>> GetByRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .Where(u => u.Role == role)
            .OrderBy(u => u.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _context.Usuarios.AddAsync(usuario, cancellationToken);
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _context.Usuarios.Update(usuario);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var usuario = await GetByIdAsync(id, cancellationToken);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
        }
    }

    public async Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email.Value == email, cancellationToken);
    }
}