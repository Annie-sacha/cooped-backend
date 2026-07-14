namespace COOPED.Application.DTOs.Frais;

public class FraisDto
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Montant { get; set; }
    public string Type { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int? PretId { get; set; }
}