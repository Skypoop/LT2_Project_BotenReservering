using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class RoleRepository : DatabaseConnection, IRoleRepository
    {
        public RoleRepository()
        {
        }

        public static async Task<RoleRepository> CreateAsync()
        {
            RoleRepository repo = new RoleRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS Role (
                    [Name] VARCHAR(50) NOT NULL PRIMARY KEY UNIQUE)");

            return repo;
        }

        public async Task<Role> Add(Role item)
        {
            string insertQuery = @"INSERT INTO Role(Name) VALUES(@Name)";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Name", item.Name);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return item;
        }

        public async Task<Role?> Get(string name)
        {
            Role? role = null;
            string selectQuery = "SELECT Name FROM Role WHERE Name = @Name";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role = new Role(reader.GetString(0));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return role;
        }

        public async Task<List<Role>> GetAll()
        {
            List<Role> roleList = new List<Role>();
            string selectQuery = "SELECT Name FROM Role";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roleList.Add(new Role(reader.GetString(0)));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return roleList;
        }
    }
}
