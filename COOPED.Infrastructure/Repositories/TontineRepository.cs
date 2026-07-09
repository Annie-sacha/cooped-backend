using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;
using COOPED.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace COOPED.Infrastructure.Repositories;

public class TontineRepository : GenericRepository<Tontine>, ITontineRepository
{
    public TontineRepository(CoopedDbContext context) : base(context) { }

    public async Task<List<Tontine>> GetByClientIdAsync(int clientId)
    {
        return await _context.Tontines
            .Where(t => t.ClientId == clientId)
            .ToListAsync();
    }

    public async Task<Tontine?> GetWithCotisationsAsync(int tontineId)
    {
        return await _context.Tontines
            .Include(t => t.Cotisations)
            .FirstOrDefaultAsync(t => t.Numero == tontineId);
    }
}