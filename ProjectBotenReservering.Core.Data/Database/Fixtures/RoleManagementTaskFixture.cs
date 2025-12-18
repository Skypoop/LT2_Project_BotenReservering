using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Fixtures;

public static class RoleManagementTaskFixture
{
    public static readonly List<RoleManagementTask> RoleManagementTasks = new()
    {
        new RoleManagementTask("Lid", 1),
        new RoleManagementTask("WedstrijdCommissaris", 2),
    };
}