namespace COOPED.Application.DTOs.Retrait;

public class RetraitDto
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal MontantTotal { get; set; }
    public string? Motif { get; set; }
    public int ClientId { get; set; }
}