public class Achat : Transaction
{
    public string Article { get; set; } = string.Empty;
    public int? TontineId { get; set; }
    public Tontine? Tontine { get; set; }
}