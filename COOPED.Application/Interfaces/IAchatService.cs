using COOPED.Application.DTOs.Achat;

namespace COOPED.Application.Interfaces;

public interface IAchatService
{
    Task<AchatDto> CreerAsync(CreerAchatRequest request);
    Task<List<AchatDto>> ObtenirParClientAsync(int clientId);
}