namespace COOPED.Application.DTOs.Tontine;

public class CaseCarnetDto
{
    public int Position { get; set; }
    public DateOnly? Date { get; set; }
    public bool Remplie { get; set; }
}