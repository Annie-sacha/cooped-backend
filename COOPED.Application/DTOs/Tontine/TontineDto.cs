namespace COOPED.Application.DTOs.Tontine;

public class TontineDto
{
    public int Numero { get; set; }
    public decimal Mise { get; set; }
    public int NbreMise { get; set; }
    public DateOnly DateCreation { get; set; }
}