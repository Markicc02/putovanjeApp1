namespace putovanjeApp1.Models;

public class Destinacija
{

<<<<<<< HEAD
    public Guid? guid { get; set; } = Guid.NewGuid();
=======
    public Guid guid { get; set; } = Guid.NewGuid();

>>>>>>> upstream/main
    public string? ime { get; set; }
    public string?  drzava { get; set; }
    public string? opis { get; set; }
    public List<string> tagovi { get; set; } = new();
}
