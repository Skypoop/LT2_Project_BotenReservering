using System.Data;
using ProjectBotenReservering.Core.Data.Database.Fixtures;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Seeders;

public class RoleManagementTaskSeeder : IDatabaseSeeder
{
    public int Order => 5;
    public void Seed(IDbConnection connection)
    {
        if (!connection.IsTableEmpty("Role_ManagementTask")) return;

        List<RoleManagementTask> roleManagementTasks = RoleManagementTaskFixture.RoleManagementTasks;
        
        foreach (RoleManagementTask roleManagementTask in roleManagementTasks)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Role_ManagementTask(Role_Id, ManagementTask_Id) VALUES(@Role_Id, @ManagementTask_Id)";
                command.AddParameter("@Role_Id", roleManagementTask.RoleId);
                command.AddParameter("@ManagementTask_Id", roleManagementTask.ManagementTaskId);
                command.ExecuteNonQuery();
            }
        }
    }
}