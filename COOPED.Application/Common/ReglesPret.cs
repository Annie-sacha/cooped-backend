using COOPED.Domain.Entities;

namespace COOPED.Application.Common;

public static class ReglesPret
{
    public static (int dureeTontine, int joursRemboursement, decimal coefFrais, int coefPret) GetParametres(TypePret type)
        => type switch
        {
            TypePret.Quinzaine => (dureeTontine: 16, joursRemboursement: 15, coefFrais: 1m, coefPret: 30),
            TypePret.Mensuel   => (dureeTontine: 31, joursRemboursement: 30, coefFrais: 1.5m, coefPret: 60),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
}