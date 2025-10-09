namespace putovanjeApp1.Models;

public class Aktivnost
{
    public Guid? guid { get; set; }
    public string? naziv { get; set; }
    public string? kategorija { get; set; }  // sport, kultura, hrana...
}