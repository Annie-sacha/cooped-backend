using COOPED.Application.DTOs.Auth;

namespace COOPED.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    string HashMotDePasse(string motDePasse);
}