using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;
using COOPED.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace COOPED.Infrastructure.Repositories;

public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    public ClientRepository(CoopedDbContext context) : base(context) { }

    public async Task<List<Client>> GetByPromoteurIdAsync(int promoteurId)
    {
        return await _context.Clients
            .Where(c => c.PromoteurId == promoteurId)
            .ToListAsync();
    }

    public async Task<Client?> GetWithDetailsAsync(int clientId)
    {
        return await _context.Clients
            .Include(c => c.Tontines)
            .Include(c => c.Retraits)
            .Include(c => c.Prets)
            .Include(c => c.Achats)
            .FirstOrDefaultAsync(c => c.Id == clientId);
    }
}