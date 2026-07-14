namespace COOPED.Application.DTOs.Client;

public class CreateClientDto
{
    public string NomCli { get; set; } = string.Empty;
    public string PrenomCli { get; set; } = string.Empty;
    public string? Sexe { get; set; }
    public string? Quartier { get; set; }
    public string? Profession { get; set; }
    public string? Telephone { get; set; }
    public string? NomPersonneAPrevenir { get; set; }
    public string? TelPersonneAPrevenir { get; set; }
    public int PromoteurId { get; set; }
}