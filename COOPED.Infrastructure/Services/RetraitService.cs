using COOPED.Application.DTOs.Retrait;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;
using COOPED.Domain.Enums;

namespace COOPED.Infrastructure.Services;

public class RetraitService : IRetraitService
{
    private readonly IGenericRepository<Retrait> _retraitRepository;

    public RetraitService(IGenericRepository<Retrait> retraitRepository)
    {
        _retraitRepository = retraitRepository;
    }

    public async Task<RetraitDto> CreerAsync(CreerRetraitRequest request)
    {
        var retrait = new Retrait
        {
            ClientId = request.ClientId,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Montant = request.MontantTotal,
            MontantTotal = request.MontantTotal,
            Motif = request.Motif,
            DateDemande = request.DateDemande,
            StatutValidation = StatutValidation.Valide
        };

        await _retraitRepository.AddAsync(retrait);
        await _retraitRepository.SaveChangesAsync();

        return VersDto(retrait);
    }

    public async Task<List<RetraitDto>> ObtenirParClientAsync(int clientId)
    {
        var retraits = await _retraitRepository.GetAllAsync();
        return retraits.Where(r => r.ClientId == clientId).Select(VersDto).ToList();
    }

    private static RetraitDto VersDto(Retrait r) => new()
    {
        Id = r.Id,
        Date = r.Date,
        MontantTotal = r.MontantTotal,
        Motif = r.Motif,
        ClientId = r.ClientId
    };
}