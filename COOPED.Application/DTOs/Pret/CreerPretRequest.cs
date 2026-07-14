using COOPED.Domain.Enums;

namespace COOPED.Application.DTOs.Pret;

public class CreerPretRequest
{
    public int ClientId { get; set; }
    public decimal MontantMise { get; set; }
    public TypePret Type { get; set; }
}