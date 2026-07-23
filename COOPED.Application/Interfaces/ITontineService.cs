using COOPED.Application.DTOs.Tontine;
using COOPED.Domain.Entities;

namespace COOPED.Application.Interfaces;

public interface ITontineService
{
    Task<TontineDto> CreerTontineNormaleAsync(int clientId, decimal mise, int nbreMise = 31);
    Task<CotisationResultDto> AjouterCotisationAsync(int tontineId, decimal montant, int nbreMise = 1);
    Task<CarnetDto> ObtenirCarnetAsync(int tontineId);
    Task<List<TontineDto>> ObtenirParClientAsync(int clientId);
}