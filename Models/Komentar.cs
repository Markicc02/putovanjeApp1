namespace putovanjeApp1.Models;

public class Komentar
<<<<<<< HEAD
{

    public Guid? guid { get; set; } = Guid.NewGuid();
=======

    public Guid guid { get; set; } = Guid.NewGuid();
>>>>>>> upstream/main

    public string? tekst { get; set; }
    public int ocena { get; set; }   // 1-5
    public string? userGuid { get; set; } // ko je napisao
    public string? atrakcijaGuid { get; set; } // na koju atrakciju se odnosi
}
