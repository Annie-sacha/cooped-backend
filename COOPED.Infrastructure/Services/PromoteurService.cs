using COOPED.Application.DTOs.Promoteur;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;

namespace COOPED.Infrastructure.Services;

public class PromoteurService : IPromoteurService
{
    private readonly IGenericRepository<Promoteur> _promoteurRepository;
    private readonly IAuthService _authService;

    public PromoteurService(IGenericRepository<Promoteur> promoteurRepository, IAuthService authService)
    {
        _promoteurRepository = promoteurRepository;
        _authService = authService;
    }

    public async Task<PromoteurDto> CreerAsync(CreatePromoteurDto dto)
    {
        var promoteur = new Promoteur
        {
            Nom = dto.Nom,
            Telephone = dto.Telephone,
            Email = dto.Email,
            // hash (BCrypt) lors de la mise en place de l'authentification
            MotDePasseHash = _authService.HashMotDePasse(dto.MotDePasse)

        };

        await _promoteurRepository.AddAsync(promoteur);
        await _promoteurRepository.SaveChangesAsync();

        return VersDto(promoteur);
    }

    public async Task<List<PromoteurDto>> ObtenirTousAsync()
    {
        var promoteurs = await _promoteurRepository.GetAllAsync();
        return promoteurs.Select(VersDto).ToList();
    }

    public async Task<PromoteurDto?> ObtenirParIdAsync(int id)
    {
        var promoteur = await _promoteurRepository.GetByIdAsync(id);
        return promoteur is null ? null : VersDto(promoteur);
    }

    public async Task<bool> ModifierAsync(int id, UpdatePromoteurDto dto)
    {
        var promoteur = await _promoteurRepository.GetByIdAsync(id);
        if (promoteur is null) return false;

        promoteur.Nom = dto.Nom;
        promoteur.Telephone = dto.Telephone;
        promoteur.Email = dto.Email;

        _promoteurRepository.Update(promoteur);
        return await _promoteurRepository.SaveChangesAsync();
    }

    public async Task<bool> SupprimerAsync(int id)
    {
        var promoteur = await _promoteurRepository.GetByIdAsync(id);
        if (promoteur is null) return false;

        _promoteurRepository.Delete(promoteur);
        return await _promoteurRepository.SaveChangesAsync();
    }

    private static PromoteurDto VersDto(Promoteur p) => new()
    {
        Id = p.Id,
        Nom = p.Nom,
        Telephone = p.Telephone,
        Email = p.Email
    };
}