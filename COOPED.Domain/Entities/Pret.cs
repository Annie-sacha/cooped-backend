using COOPED.Domain.Enums;

namespace COOPED.Domain.Entities;

public enum TypePret { Mensuel, Quinzaine }

public class Pret : Transaction
{
    public TypePret Type { get; set; }
    public int DureeRemboursement { get; set; }     // 15 ou 30 jours (durée initiale)
    public DateOnly DateEcheance { get; set; }       // échéance courante, repoussée à chaque pénalité
    public StatutPret Statut { get; set; } = StatutPret.EnCours;

    public int? TontineId { get; set; }
    public Tontine? Tontine { get; set; }

    public List<Penalite> Penalites { get; set; } = new();
}