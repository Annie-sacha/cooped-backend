namespace COOPED.Application.DTOs.Retrait;

public class CreerRetraitRequest
{
    public int ClientId { get; set; }
    public decimal MontantTotal { get; set; }
    public string? Motif { get; set; }
    public DateOnly? DateDemande { get; set; }
}