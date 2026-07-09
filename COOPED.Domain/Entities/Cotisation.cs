namespace COOPED.Domain.Entities;

public class Cotisation : Transaction
{
    public int NbreMise { get; set; }
    public int Position { get; set; }
    public int TontineId { get; set; }
    public Tontine? Tontine { get; set; }
}
