namespace COOPED.Domain.Entities;

public class Penalite
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Montant { get; set; }   // = le frais rejoué (1x ou 1.5x la mise selon le type de prêt)

    public int PretId { get; set; }
    public Pret? Pret { get; set; }
}