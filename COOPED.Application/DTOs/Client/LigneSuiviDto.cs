namespace COOPED.Application.DTOs.Client;

public class LigneSuiviDto
{
    public DateOnly Date { get; set; }
    public string Designation { get; set; } = string.Empty;
    public decimal Entree { get; set; }
    public decimal Sortie { get; set; }
    public decimal Solde { get; set; }
}