namespace COOPED.Application.DTOs.Tontine;

public class AjouterCotisationRequest
{
    public decimal Montant { get; set; }
    public int NbreMise { get; set; } = 1;
}