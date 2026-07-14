namespace COOPED.Application.DTOs.Achat;

public class CreerAchatRequest
{
    public int ClientId { get; set; }
    public decimal Montant { get; set; }
    public string Article { get; set; } = string.Empty;
    public int? TontineId { get; set; }   // optionnel : si l'achat vient d'une tontine dédiée
}