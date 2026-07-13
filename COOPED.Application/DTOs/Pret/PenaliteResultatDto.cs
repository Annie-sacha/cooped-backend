namespace COOPED.Application.DTOs.Pret;

public class PenaliteResultDto
{
    public bool PenaliteAppliquee { get; set; }
    public decimal? MontantPenalite { get; set; }
    public DateOnly? NouvelleEcheance { get; set; }
    public string Message { get; set; } = string.Empty;
}