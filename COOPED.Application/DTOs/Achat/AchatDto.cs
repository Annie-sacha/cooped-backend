namespace COOPED.Application.DTOs.Achat;

public class AchatDto
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Montant { get; set; }
    public string Article { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int? TontineId { get; set; }
}