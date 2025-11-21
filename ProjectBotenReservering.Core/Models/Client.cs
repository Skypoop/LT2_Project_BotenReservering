namespace ProjectBotenReservering.Core.Models;

public class Client
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int ScullLevel { get; set; }
    public int RoeiLevel { get; set; }
    public string? Club { get; set; }
    public bool Approved { get; set; }
    public string PasswordHash { get; set; }

    public Client(string fullName, string email, int scullLevel, int roeiLevel, string? club, bool approved, string passwordHash, int id = 0)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        ScullLevel = scullLevel;
        RoeiLevel = roeiLevel;
        Club = club;
        Approved = approved;
        PasswordHash = passwordHash;
    }
}