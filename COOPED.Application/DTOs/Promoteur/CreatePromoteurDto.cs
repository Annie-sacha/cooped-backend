namespace COOPED.Application.DTOs.Promoteur;

public class CreatePromoteurDto
{
    public string Nom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasse { get; set; } = string.Empty;
}