using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class RoleRepository : DatabaseConnection, IRoleRepository
    {
        public RoleRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Role (
                            [Name] VARCHAR(50) NOT NULL PRIMARY KEY UNIQUE)");
        }

        public Role Add(Role item)
        {
            string insertQuery = @"INSERT INTO Role(Name) VALUES(@Name)";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public Role? Get(string name)
        {
            Role? role = null;
            string selectQuery = "SELECT Name FROM Role WHERE Name = @Name";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return role;
        }

        public List<Role> GetAll()
        {
            var roleList = new List<Role>();
            string selectQuery = "SELECT Name FROM Role";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roleList.Add(new Role(reader.GetString(0)));
                    }
                }
            }

            CloseConnection();
            return roleList;
        }
    }
}

