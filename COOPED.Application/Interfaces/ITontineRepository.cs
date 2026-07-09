using COOPED.Domain.Entities;

namespace COOPED.Application.Interfaces;

public interface ITontineRepository : IGenericRepository<Tontine>
{
    Task<List<Tontine>> GetByClientIdAsync(int clientId);
    Task<Tontine?> GetWithCotisationsAsync(int tontineId);
}