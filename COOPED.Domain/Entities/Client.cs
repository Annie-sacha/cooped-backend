namespace COOPED.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public string NomCli { get; set; } = string.Empty;
    public string PrenomCli { get; set; } = string.Empty;
    public string? Sexe { get; set; }
    public string? Quartier { get; set; }
    public string? Profession { get; set; }
    public string? Telephone { get; set; }
    public DateOnly DateCarnet { get; set; }

    // Personne a prevenir (npv)
    public string? NomPersonneAPrevenir { get; set; }
    public string? TelPersonneAPrevenir { get; set; }

    public int PromoteurId { get; set; }
    public Promoteur? Promoteur { get; set; }

    public List<Tontine> Tontines { get; set; } = new();
    public List<Retrait> Retraits { get; set; } = new();
    public List<Pret> Prets { get; set; } = new();
    public List<Frais> FraisList { get; set; } = new();
    public List<Achat> Achats { get; set; } = new();
}