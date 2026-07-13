using COOPED.Application.Common;
using COOPED.Application.DTOs.Pret;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;
using COOPED.Domain.Enums;

namespace COOPED.Infrastructure.Services;

public class PenaliteService : IPenaliteService
{
    private readonly IGenericRepository<Pret> _pretRepository;
    private readonly IGenericRepository<Penalite> _penaliteRepository;

    public PenaliteService(
        IGenericRepository<Pret> pretRepository,
        IGenericRepository<Penalite> penaliteRepository)
    {
        _pretRepository = pretRepository;
        _penaliteRepository = penaliteRepository;
    }

    public async Task<PenaliteResultDto> VerifierEtAppliquerAsync(int pretId)
    {
        var pret = await _pretRepository.GetByIdAsync(pretId);
        if (pret is null)
            throw new InvalidOperationException("Prêt introuvable.");

        // Si déjà remboursé, rien à faire
        if (pret.Statut == StatutPret.Rembourse)
        {
            return new PenaliteResultDto
            {
                PenaliteAppliquee = false,
                Message = "Le prêt est déjà remboursé."
            };
        }

        var aujourdHui = DateOnly.FromDateTime(DateTime.Now);

        // Échéance pas encore atteinte : rien à faire
        if (aujourdHui <= pret.DateEcheance)
        {
            return new PenaliteResultDto
            {
                PenaliteAppliquee = false,
                Message = "Le prêt est encore dans les délais."
            };
        }

        // Échéance dépassée sans remboursement : on applique la pénalité
        var (_, joursRemboursement, coefFrais, _) = ReglesPret.GetParametres(pret.Type);
        decimal montantMiseInitiale = pret.Montant / ReglesPret.GetParametres(pret.Type).coefPret;
        decimal montantPenalite = montantMiseInitiale * coefFrais;

        var penalite = new Penalite
        {
            PretId = pret.Id,
            Date = aujourdHui,
            Montant = montantPenalite
        };
        await _penaliteRepository.AddAsync(penalite);

        // Repousser l'échéance et marquer le prêt en retard
        pret.DateEcheance = pret.DateEcheance.AddDays(joursRemboursement);
        pret.Statut = StatutPret.EnRetard;
        _pretRepository.Update(pret);

        await _penaliteRepository.SaveChangesAsync();

        return new PenaliteResultDto
        {
            PenaliteAppliquee = true,
            MontantPenalite = montantPenalite,
            NouvelleEcheance = pret.DateEcheance,
            Message = $"Pénalité de {montantPenalite} appliquée. Nouvelle échéance : {pret.DateEcheance}."
        };
    }
}