using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientRoleRepository : DatabaseConnection, IClientRoleRepository
    {
        public ClientRoleRepository()
        {
        }

        public static async Task<ClientRoleRepository> CreateAsync()
        {
            ClientRoleRepository repo = new ClientRoleRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS Client_Role (
                    [Role_Name] VARCHAR(50) NOT NULL,
                    [Client_Id] INT NOT NULL,
                    PRIMARY KEY (Role_Name, Client_Id),
                    FOREIGN KEY (Role_Name) REFERENCES Role(Name),
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id))");

            return repo;
        }

        public async Task<ClientRole> Add(ClientRole item)
        {
            string insertQuery = @"INSERT INTO Client_Role(Role_Name, Client_Id) 
                                   VALUES(@RoleName, @ClientId)";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@RoleName", item.RoleName);
                    command.Parameters.AddWithValue("@ClientId", item.ClientId);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return item;
        }

        public async Task<List<ClientRole>> GetByClientId(int clientId)
        {
            List<ClientRole> list = new List<ClientRole>();
            string selectQuery = "SELECT Role_Name, Client_Id FROM Client_Role WHERE Client_Id = @ClientId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ClientRole(reader.GetString(0), reader.GetInt32(1)));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return list;
        }

        public async Task<List<ClientRole>> GetByRoleName(string roleName)
        {
            List<ClientRole> list = new List<ClientRole>();
            string selectQuery = "SELECT Role_Name, Client_Id FROM Client_Role WHERE Role_Name = @RoleName";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@RoleName", roleName);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ClientRole(reader.GetString(0), reader.GetInt32(1)));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return list;
        }

        public async Task Delete(string roleName, int clientId)
        {
            string deleteQuery = "DELETE FROM Client_Role WHERE Role_Name = @RoleName AND Client_Id = @ClientId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(deleteQuery, Connection))
                {
                    command.Parameters.AddWithValue("@RoleName", roleName);
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }
        }
    }
}
