using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Fixtures;

public static class ClientRoleFixture
{
    public static readonly List<ClientRole> ClientRoles = new()
    {
        new ClientRole("Lid", 1),
        new ClientRole("Lid", 2),
        new ClientRole("Lid", 3),
        new ClientRole("Lid", 4),
        new ClientRole("Lid", 5),
        new ClientRole("Lid", 6),
        new ClientRole("Lid", 7),
        new ClientRole("Lid", 8),
        new ClientRole("Lid", 9),
        new ClientRole("Lid", 10),
        new ClientRole("Lid", 11),
        new ClientRole("Lid", 12),
        new ClientRole("Lid", 13),
        new ClientRole("Lid", 14),
        new ClientRole("Lid", 15),
        new ClientRole("Lid", 16),
        
        new ClientRole("Gast", 17),
        new ClientRole("Gast", 18),
        new ClientRole("Gast", 19),
        new ClientRole("Nieuw Lid", 20),

        
        new ClientRole("WedstrijdCommissaris", 3),
        new ClientRole("WedstrijdCommissaris", 5),
    };
}