using System.Collections.Generic;
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
            if (!IsTableEmpty(connection)) return;

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

        private bool IsTableEmpty(IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Role";
                long count = Convert.ToInt64(command.ExecuteScalar());
                return count == 0;
            }
        }
    }
}