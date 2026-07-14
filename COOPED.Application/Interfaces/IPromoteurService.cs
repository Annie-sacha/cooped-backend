using COOPED.Application.DTOs.Promoteur;

namespace COOPED.Application.Interfaces;

public interface IPromoteurService
{
    Task<PromoteurDto> CreerAsync(CreatePromoteurDto dto);
    Task<List<PromoteurDto>> ObtenirTousAsync();
    Task<PromoteurDto?> ObtenirParIdAsync(int id);
    Task<bool> ModifierAsync(int id, UpdatePromoteurDto dto);
    Task<bool> SupprimerAsync(int id);
}