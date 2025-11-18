namespace ProjectBotenReservering.Core.Models;

public class Client
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string EmailAddress { get; set; }
    public Role Role { get; set; } = Role.None;
    public string PasswordHash { get; set; }

    public Client(int id, string name, string emailAddress, string passwordHash)
    {
        Id = id;
        FullName = name;
        EmailAddress = emailAddress;
        PasswordHash = passwordHash;
    }
}