using COOPED.Application.DTOs.Client;

namespace COOPED.Application.Interfaces;

public interface IClientService
{
    Task<ClientDto> CreerAsync(CreateClientDto dto);
    Task<List<ClientDto>> ObtenirTousAsync();
    Task<ClientDto?> ObtenirParIdAsync(int id);
    Task<List<ClientDto>> ObtenirParPromoteurAsync(int promoteurId);
    Task<bool> ModifierAsync(int id, UpdateClientDto dto);
    Task<bool> SupprimerAsync(int id);
}