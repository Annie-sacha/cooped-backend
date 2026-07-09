namespace COOPED.Domain.Entities;

public class Promoteur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;

    // Un promoteur gere plusieurs clients 
    public List<Client> Clients { get; set; } = new();    
}