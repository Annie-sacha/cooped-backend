namespace COOPED.Application.DTOs.Pret;

public class PretResultDto
{
    public int PretId { get; set; }
    public int TontineId { get; set; }
    public decimal MontantMise { get; set; }
    public decimal Frais { get; set; }
    public decimal MontantPrete { get; set; }
    public DateOnly DateEcheance { get; set; }
}