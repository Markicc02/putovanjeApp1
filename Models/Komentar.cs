namespace putovanjeApp1.Models;

public class Komentar
{
    public Guid? guid { get; set; } = Guid.NewGuid();
    public string? tekst { get; set; }
    public int ocena { get; set; }   // 1-5
    public string? userId { get; set; } // ko je napisao
    public string? atrakcijaId { get; set; } // na koju atrakciju se odnosi
}
