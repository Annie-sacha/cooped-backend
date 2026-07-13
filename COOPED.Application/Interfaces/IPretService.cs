using COOPED.Application.DTOs.Pret;
using COOPED.Domain.Entities;
using COOPED.Domain.Enums;

namespace COOPED.Application.Interfaces;

public interface IPretService
{
    Task<PretResultDto> CreerPretAsync(int clientId, decimal montantMise, TypePret type);
}