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

        new ClientRole("WedstrijdCommissaris", 3),
        new ClientRole("WedstrijdCommissaris", 5),
    };
}