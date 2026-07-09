using COOPED.Domain.Entities;

namespace COOPED.Application.Interfaces;

public interface IClientRepository : IGenericRepository<Client>
{
    Task<List<Client>> GetByPromoteurIdAsync(int promoteurId);
    Task<Client?> GetWithDetailsAsync(int clientId);
}