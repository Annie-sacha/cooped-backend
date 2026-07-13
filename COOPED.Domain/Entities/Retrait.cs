using COOPED.Domain.Enums;

namespace COOPED.Domain.Entities;

public class Retrait : Transaction
{
    public string? Motif { get; set; }
    public decimal MontantTotal { get; set; }

    public DateOnly? DateDemande { get; set; }   // souvent absente en pratique
    public StatutValidation StatutValidation { get; set; } = StatutValidation.Valide;
    // Date (hérité de Transaction) = date d'exécution
}