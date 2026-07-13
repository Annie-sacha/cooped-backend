using COOPED.Application.DTOs.Pret;

namespace COOPED.Application.Interfaces;

public interface IPenaliteService
{
    Task<PenaliteResultDto> VerifierEtAppliquerAsync(int pretId);
}