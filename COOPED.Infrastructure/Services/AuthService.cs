using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using COOPED.Application.DTOs.Auth;
using COOPED.Application.Interfaces;
using COOPED.Domain.Enums;
using COOPED.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace COOPED.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly CoopedDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(CoopedDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // 1. Chercher d'abord parmi les administrateurs
        var admin = await _context.Administrateurs
            .FirstOrDefaultAsync(a => a.Email == request.Email);

        if (admin is not null && BCrypt.Net.BCrypt.Verify(request.MotDePasse, admin.MotDePasseHash))
        {
            return GenererToken(admin.Id, admin.Nom, RoleUtilisateur.Administrateur);
        }

        // 2. Sinon, chercher parmi les promoteurs
        var promoteur = await _context.Promoteurs
            .FirstOrDefaultAsync(p => p.Email == request.Email);

        if (promoteur is not null && BCrypt.Net.BCrypt.Verify(request.MotDePasse, promoteur.MotDePasseHash))
        {
            return GenererToken(promoteur.Id, promoteur.Nom, RoleUtilisateur.Promoteur);
        }

        // Aucune correspondance
        return null;
    }

    public string HashMotDePasse(string motDePasse) => BCrypt.Net.BCrypt.HashPassword(motDePasse);

    private LoginResponse GenererToken(int utilisateurId, string nom, RoleUtilisateur role)
    {
        var cle = _configuration["Jwt:Cle"]!;
        var dureeHeures = int.Parse(_configuration["Jwt:DureeExpirationHeures"] ?? "12");
        var expiration = DateTime.UtcNow.AddHours(dureeHeures);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, utilisateurId.ToString()),
            new Claim(ClaimTypes.Name, nom),
            new Claim(ClaimTypes.Role, role.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cle)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UtilisateurId = utilisateurId,
            Nom = nom,
            Role = role,
            ExpirationToken = expiration
        };
    }
}