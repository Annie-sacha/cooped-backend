namespace COOPED.Domain.Entities;


public abstract class Transaction
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Montant { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; }
}


// Transaction.cs
/*public abstract class Transaction
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Mise { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; }
}*/











