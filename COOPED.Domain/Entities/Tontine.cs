namespace COOPED.Domain.Entities;

public enum TypeTontine
{
    Normale,
    Pret,
    Achat
}

public class Tontine
{
    public int Numero { get; set; }
    public DateOnly DateCreation { get; set; }
    public DateOnly? DateFin { get; set; }
    public decimal Mise { get; set; }
    public int NbreMise { get; set; }
    public TypeTontine Type { get; set; } = TypeTontine.Normale;

    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public List<Cotisation> Cotisations { get; set; } = new();

    public int? PretId { get; set; }
    public Pret? Pret { get; set; }

    public int? AchatId { get; set; }
    public Achat? Achat { get; set; }
}