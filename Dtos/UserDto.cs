namespace putovanjeApp1.Dtos
{
    public class UserDto
    {
        public Guid? Guid { get; set; }
        public string? Ime { get; set; }
        public string? Email { get; set; }
        public List<string> Interesovanja { get; set; } = new();

        public List<Guid> Putovanja { get; set; } = new(); // korisnik ima više putovanja
       
    }
}
