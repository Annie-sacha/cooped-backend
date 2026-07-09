using COOPED.Application.DTOs.Client;

namespace COOPED.Application.Interfaces;

public interface ISuiviService
{
    Task<List<LigneSuiviDto>> GenererSuiviAsync(int clientId);
}