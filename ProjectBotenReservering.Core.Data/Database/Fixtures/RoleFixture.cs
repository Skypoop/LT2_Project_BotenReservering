using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Fixtures;

public static class RoleFixture
{
    public static readonly List<Role> Roles = new()
    {
        new Role("Lid"),
        new Role("Nieuw Lid"),
        new Role("Gast"),
        new Role("WedstrijdCommissaris"),
    };
}