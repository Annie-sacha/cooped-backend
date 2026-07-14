namespace COOPED.Application.DTOs.Tontine;

public class CarnetDto
{
    public int TontineId { get; set; }
    public decimal Mise { get; set; }
    public int NbreMise { get; set; }
    public bool Cloturee { get; set; }
    public List<CaseCarnetDto> Cases { get; set; } = new();
}