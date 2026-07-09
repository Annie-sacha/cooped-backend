namespace COOPED.Domain.Entities;

// Transaction.cs
/*public abstract class Transaction
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Mise { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; }
}*/



public abstract class Transaction
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Montant { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; }
}

// Cotisation.cs
public class Cotisation : Transaction
{
    public int NbreMise { get; set; }
    public int Position { get; set; }
    public int TontineId { get; set; }
    public Tontine? Tontine { get; set; }
}

// Retrait.cs
public class Retrait : Transaction
{
    public string? Motif { get; set; }
    public decimal MontantTotal { get; set; }
}

// Achat.cs
public class Achat : Transaction
{
    public string Article { get; set; } = string.Empty;
    public int? TontineId { get; set; }
    public Tontine? Tontine { get; set; }
}

// Pret.cs
public enum TypePret { Mensuel, Quinzaine }

public class Pret : Transaction
{
    public TypePret Type { get; set; }
    public int DureeRemboursement { get; set; }
    public int? TontineId { get; set; }
    public Tontine? Tontine { get; set; }
}

// Frais.cs
public class Frais : Transaction
{
    public string Type { get; set; } = string.Empty;
    public int? PretId { get; set; }
    public Pret? Pret { get; set; }
}
