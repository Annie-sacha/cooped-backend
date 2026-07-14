using COOPED.Application.DTOs.Achat;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;

namespace COOPED.Infrastructure.Services;

public class AchatService : IAchatService
{
    private readonly IGenericRepository<Achat> _achatRepository;

    public AchatService(IGenericRepository<Achat> achatRepository)
    {
        _achatRepository = achatRepository;
    }

    public async Task<AchatDto> CreerAsync(CreerAchatRequest request)
    {
        var achat = new Achat
        {
            ClientId = request.ClientId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Montant = request.Montant,
            Article = request.Article,
            TontineId = request.TontineId
        };

        await _achatRepository.AddAsync(achat);
        await _achatRepository.SaveChangesAsync();

        return VersDto(achat);
    }

    public async Task<List<AchatDto>> ObtenirParClientAsync(int clientId)
    {
        var achats = await _achatRepository.GetAllAsync();
        return achats.Where(a => a.ClientId == clientId).Select(VersDto).ToList();
    }

    private static AchatDto VersDto(Achat a) => new()
    {
        Id = a.Id,
        Date = a.Date,
        Montant = a.Montant,
        Article = a.Article,
        ClientId = a.ClientId,
        TontineId = a.TontineId
    };
}