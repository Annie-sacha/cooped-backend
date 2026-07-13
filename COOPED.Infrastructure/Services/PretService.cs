using COOPED.Application.Common;
using COOPED.Application.DTOs.Pret;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;
using COOPED.Domain.Enums;

namespace COOPED.Infrastructure.Services;


// le service doit passer par les repository pour communiquer avec coopedDbContext et parvenir à la BD
public class PretService : IPretService
{
    private readonly IClientRepository _clientRepository;
    private readonly ITontineRepository _tontineRepository;
    private readonly IGenericRepository<Pret> _pretRepository;
    private readonly IGenericRepository<Frais> _fraisRepository;

    public PretService(
        IClientRepository clientRepository,
        ITontineRepository tontineRepository,
        IGenericRepository<Pret> pretRepository,
        IGenericRepository<Frais> fraisRepository)
    {
        _clientRepository = clientRepository;
        _tontineRepository = tontineRepository;
        _pretRepository = pretRepository;
        _fraisRepository = fraisRepository;
    }

    public async Task<PretResultDto> CreerPretAsync(int clientId, decimal montantMise, TypePret type)
    {
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client is null)
            throw new InvalidOperationException("Client introuvable.");

        var (dureeTontine, joursRemboursement, coefFrais, coefPret) = ReglesPret.GetParametres(type);
        var aujourdHui = DateOnly.FromDateTime(DateTime.Now);

        // 1. Ouvrir la tontine spéciale liée au prêt
        var tontine = new Tontine
        {
            ClientId = clientId,
            Mise = montantMise,
            NbreMise = dureeTontine,
            DateCreation = aujourdHui,
            Type = TypeTontine.Pret
        };
        await _tontineRepository.AddAsync(tontine);
        await _tontineRepository.SaveChangesAsync();   // nécessaire pour récupérer tontine.Numero

        // 2. Calculer frais et montant prêté selon le type
        decimal frais = montantMise * coefFrais;
        decimal montantPrete = montantMise * coefPret;
        var dateEcheance = aujourdHui.AddDays(joursRemboursement);

        // 3. Créer le prêt
        var pret = new Pret
        {
            ClientId = clientId,
            Date = aujourdHui,
            Montant = montantPrete,
            Type = type,
            DureeRemboursement = joursRemboursement,
            DateEcheance = dateEcheance,
            Statut = StatutPret.EnCours,
            TontineId = tontine.Numero
        };
        await _pretRepository.AddAsync(pret);
        await _pretRepository.SaveChangesAsync();

        // 4. Créer le frais associé (l'intérêt prélevé)
        var fraisEntity = new Frais
        {
            ClientId = clientId,
            Date = aujourdHui,
            Montant = frais,
            Type = type.ToString(),
            PretId = pret.Id
        };
        await _fraisRepository.AddAsync(fraisEntity);
        await _fraisRepository.SaveChangesAsync();

        return new PretResultDto
        {
            PretId = pret.Id,
            TontineId = tontine.Numero,
            MontantMise = montantMise,
            Frais = frais,
            MontantPrete = montantPrete,
            DateEcheance = dateEcheance
        };
    }
}