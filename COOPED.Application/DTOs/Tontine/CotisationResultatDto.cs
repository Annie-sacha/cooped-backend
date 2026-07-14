namespace COOPED.Application.DTOs.Tontine;

public class CotisationResultDto
{
    public int CotisationId { get; set; }
    public int Position { get; set; }
    public bool TontineCloturee { get; set; }
    public int CasesRestantes { get; set; }
}