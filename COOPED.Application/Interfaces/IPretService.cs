using COOPED.Application.DTOs.Pret;
using COOPED.Domain.Entities;

namespace COOPED.Application.Interfaces;

public interface IPretService
{
    Task<PretResultDto> CreerPretAsync(int clientId, decimal montantMise, TypePret type);
}