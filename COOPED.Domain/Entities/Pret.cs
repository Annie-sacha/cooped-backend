namespace COOPED.Domain.Entities;

public enum TypePret { Mensuel, Quinzaine }

public class Pret : Transaction
{
    public TypePret Type { get; set; }
    public int DureeRemboursement { get; set; }
    public int? TontineId { get; set; }
    public Tontine? Tontine { get; set; }
}