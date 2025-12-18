using System.Data;
using ProjectBotenReservering.Core.Data.Database.Fixtures;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Seeders
{
    public class RoleSeeder : IDatabaseSeeder
    {
        public int Order => 3;

        public void Seed(IDbConnection connection)
        {
            if (!connection.IsTableEmpty("Role")) return;

            List<Role> roles = RoleFixture.Roles;

            foreach (Role role in roles)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Role(Name) VALUES(@Name)";
                    command.AddParameter("@Name", role.Name);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}