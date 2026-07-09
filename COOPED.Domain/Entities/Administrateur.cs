namespace COOPED.Domain.Entities;

public class Administrateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;
}