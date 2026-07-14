using COOPED.Application.DTOs.Retrait;

namespace COOPED.Application.Interfaces;

public interface IRetraitService
{
    Task<RetraitDto> CreerAsync(CreerRetraitRequest request);
    Task<List<RetraitDto>> ObtenirParClientAsync(int clientId);
}