using COOPED.Application.DTOs.Client;
using COOPED.Application.Interfaces;
using COOPED.Domain.Entities;

namespace COOPED.Infrastructure.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ClientDto> CreerAsync(CreateClientDto dto)
    {
        var client = new Client
        {
            NomCli = dto.NomCli,
            PrenomCli = dto.PrenomCli,
            Sexe = dto.Sexe,
            Quartier = dto.Quartier,
            Profession = dto.Profession,
            Telephone = dto.Telephone,
            NomPersonneAPrevenir = dto.NomPersonneAPrevenir,
            TelPersonneAPrevenir = dto.TelPersonneAPrevenir,
            PromoteurId = dto.PromoteurId,
            DateCarnet = DateOnly.FromDateTime(DateTime.Now)
        };

        await _clientRepository.AddAsync(client);
        await _clientRepository.SaveChangesAsync();

        return VersDto(client);
    }

    public async Task<List<ClientDto>> ObtenirTousAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        return clients.Select(VersDto).ToList();
    }

    public async Task<ClientDto?> ObtenirParIdAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        return client is null ? null : VersDto(client);
    }

    public async Task<List<ClientDto>> ObtenirParPromoteurAsync(int promoteurId)
    {
        var clients = await _clientRepository.GetByPromoteurIdAsync(promoteurId);
        return clients.Select(VersDto).ToList();
    }

    public async Task<bool> ModifierAsync(int id, UpdateClientDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client is null) return false;

        client.NomCli = dto.NomCli;
        client.PrenomCli = dto.PrenomCli;
        client.Sexe = dto.Sexe;
        client.Quartier = dto.Quartier;
        client.Profession = dto.Profession;
        client.Telephone = dto.Telephone;
        client.NomPersonneAPrevenir = dto.NomPersonneAPrevenir;
        client.TelPersonneAPrevenir = dto.TelPersonneAPrevenir;

        _clientRepository.Update(client);
        return await _clientRepository.SaveChangesAsync();
    }

    public async Task<bool> SupprimerAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client is null) return false;

        _clientRepository.Delete(client);
        return await _clientRepository.SaveChangesAsync();
    }

    private static ClientDto VersDto(Client c) => new()
    {
        Id = c.Id,
        NomCli = c.NomCli,
        PrenomCli = c.PrenomCli,
        Sexe = c.Sexe,
        Quartier = c.Quartier,
        Profession = c.Profession,
        Telephone = c.Telephone,
        DateCarnet = c.DateCarnet,
        NomPersonneAPrevenir = c.NomPersonneAPrevenir,
        TelPersonneAPrevenir = c.TelPersonneAPrevenir,
        PromoteurId = c.PromoteurId
    };
}