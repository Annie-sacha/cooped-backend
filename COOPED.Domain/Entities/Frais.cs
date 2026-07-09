public class Frais : Transaction
{
    public string Type { get; set; } = string.Empty;
    public int? PretId { get; set; }
    public Pret? Pret { get; set; }
}
