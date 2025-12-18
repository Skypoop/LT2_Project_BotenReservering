using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Fixtures;

public static class ManagementTaskFixture
{
    public static readonly List<ManagementTask> ManagementTasks = new()
    {
        new ManagementTask(1, "Reserveren"),
        new ManagementTask(2, "Wedstrijden"),
    };
}