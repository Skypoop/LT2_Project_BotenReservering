using System.Data;
using ProjectBotenReservering.Core.Data.Database.Fixtures;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Seeders;

public class ManagementTaskSeeder : IDatabaseSeeder
{
    public int Order => 4;
    public void Seed(IDbConnection connection)
    {
        if (!connection.IsTableEmpty("ManagementTask")) return;

        List<ManagementTask> managementTasks = ManagementTaskFixture.ManagementTasks;
        
        foreach (ManagementTask managementTask in managementTasks)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO ManagementTask(Name) VALUES(@Name)";
                command.AddParameter("@Name", managementTask.Name);
                command.ExecuteNonQuery();
            }
        }
    }
}