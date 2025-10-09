namespace putovanjeApp1.Models;

public class Komentar
{
    public int? id { get; set; }
    public string? tekst { get; set; }
    public int ocena { get; set; }   // 1-5
    public string? userId { get; set; } // ko je napisao
    public string? atrakcijaId { get; set; } // na koju atrakciju se odnosi
}
