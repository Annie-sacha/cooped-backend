using COOPED.Domain.Enums;

namespace COOPED.Domain.Entities;

public class Pret : Transaction
{
    public TypePret Type { get; set; }
    public int DureeRemboursement { get; set; }
    public DateOnly DateEcheance { get; set; }
    public StatutPret Statut { get; set; } = StatutPret.EnCours;

    public DateOnly? DateDemande { get; set; }
    public StatutValidation StatutValidation { get; set; } = StatutValidation.Valide;

    public int? TontineId { get; set; }
    public Tontine? Tontine { get; set; }

    public List<Penalite> Penalites { get; set; } = new();
}