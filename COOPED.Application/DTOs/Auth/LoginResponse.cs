using COOPED.Domain.Enums;

namespace COOPED.Application.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int UtilisateurId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public RoleUtilisateur Role { get; set; }
    public DateTime ExpirationToken { get; set; }
}