namespace ProjectBotenReservering.Core.Models;

public class Client
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int ScullLevel { get; set; }
    public int SweepLevel { get; set; }
    public string? Club { get; set; }
    public bool Approved { get; set; }
    public string PasswordHash { get; set; }

    // Runtime-only flag set by the UI/viewmodel to indicate the client is under the required level for the selected boat
    public bool IsUnderqualified { get; set; }

    // Runtime-only help text shown on hover for the qualification warning
    public string QualificationHelpText { get; set; }

    public Client(string fullName, string email, int scullLevel, int sweepLevel, string? club, bool approved, string passwordHash, int id = 0)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        ScullLevel = scullLevel;
        SweepLevel = sweepLevel;
        Club = club;
        Approved = approved;
        PasswordHash = passwordHash;
        IsUnderqualified = false;
        QualificationHelpText = string.Empty;
    }
}