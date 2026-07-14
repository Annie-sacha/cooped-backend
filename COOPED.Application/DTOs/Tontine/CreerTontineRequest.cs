namespace COOPED.Application.DTOs.Tontine;

public class CreerTontineRequest
{
    public int ClientId { get; set; }
    public decimal Mise { get; set; }
    public int NbreMise { get; set; } = 31;
}