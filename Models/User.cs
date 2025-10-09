namespace putovanjeApp1.Models;

public class User
{
    public int? id { get; set; } // jedinstveni ID
    public string? ime { get; set; }
    public string? email { get; set; }
    public string? passwordHash { get; set; }
    public List<string> interesovanja { get; set; } = new();
}

