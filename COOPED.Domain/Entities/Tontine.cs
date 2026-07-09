namespace COOPED.Domain.Entities;

public class Tontine
{
    public int Numero { get; set; }
    public DateOnly DateCreation { get; set; }
    public DateOnly? DateFin { get; set; }
    public decimal Mise { get; set; }
    public int NbreMise { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public List<Cotisation> Cotisations { get; set; } = new();

    // Une tontine peut etre liee a un pret ou un achat (celui qui l'a ouverte)
    public int? PretId { get; set; }
    public Pret? Pret { get; set; }

    public int? AchatId { get; set; }
    public Achat? Achat { get; set; }
}