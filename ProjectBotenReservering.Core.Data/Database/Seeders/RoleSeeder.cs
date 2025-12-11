using System.Data;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;

namespace ProjectBotenReservering.Core.Data.Database.Seeders
{
    public class RoleSeeder : IDatabaseSeeder
    {
        public int Order => 1;

        public void Seed(IDbConnection connection)
        {
            if (!connection.IsTableEmpty("Role")) return;

            List<string> roles = new List<string> { "Lid", "Nieuw Lid", "Gast" };

            foreach (string role in roles)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Role(Name) VALUES(@Name)";
                    command.AddParameter("@Name", role);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}