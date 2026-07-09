namespace COOPED.Domain.Entities;

public class Retrait : Transaction
{
    public string? Motif { get; set; }
    public decimal MontantTotal { get; set; }
}