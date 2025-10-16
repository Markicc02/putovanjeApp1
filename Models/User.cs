namespace putovanjeApp1.Models;

public class User
{
<<<<<<< HEAD
    public Guid? guid { get; set; } = Guid.NewGuid();
=======

    public Guid? guid { get; set; } = Guid.NewGuid();

>>>>>>> upstream/main
    public string? ime { get; set; }
    public string? email { get; set; }
    public string? passwordHash { get; set; }
    public List<string> interesovanja { get; set; } = new();
}

