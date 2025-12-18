using System.Data;
using ProjectBotenReservering.Core.Data.Database.Fixtures;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Seeders;

public class ClientRoleSeeder : IDatabaseSeeder
{
    public int Order => 4;
    public void Seed(IDbConnection connection)
    {
        if (!connection.IsTableEmpty("Client_Role")) return;

        List<ClientRole> clientRoles = ClientRoleFixture.ClientRoles;
        
        foreach (ClientRole clientRole in clientRoles)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Client_Role(Role_Name, Client_Id) VALUES(@Role_Name, @Client_Id)";
                command.AddParameter("@Role_Name", clientRole.RoleName);
                command.AddParameter("@Client_Id", clientRole.ClientId);
                command.ExecuteNonQuery();
            }
        }
    }
}