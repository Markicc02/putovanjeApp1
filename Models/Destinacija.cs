namespace putovanjeApp1.Models;

public class Destinacija
{
    public int? id { get; set; }
    public string? ime { get; set; }
    public string?  drzava { get; set; }
    public string? opis { get; set; }
    public List<string> tagovi { get; set; } = new();
}
