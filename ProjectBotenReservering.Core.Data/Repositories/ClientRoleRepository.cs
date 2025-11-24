using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientRoleRepository : DatabaseConnection, IClientRoleRepository
    {
        public ClientRoleRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Client_Role (
                            [Role_Name] VARCHAR(50) NOT NULL,
                            [Client_Id] INT NOT NULL,
                            PRIMARY KEY (Role_Name, Client_Id),
                            FOREIGN KEY (Role_Name) REFERENCES Role(Name),
                            FOREIGN KEY (Client_Id) REFERENCES Client(Id))");
        }

        public async Task<ClientRole> Add(ClientRole item)
        {
            string insertQuery = @"INSERT INTO Client_Role(Role_Name, Client_Id) 
                                   VALUES(@RoleName, @ClientId)";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@RoleName", item.RoleName);
                command.Parameters.AddWithValue("@ClientId", item.ClientId);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public async Task<List<ClientRole>> GetByClientId(int clientId)
        {
            var list = new List<ClientRole>();
            string selectQuery = "SELECT Role_Name, Client_Id FROM Client_Role WHERE Client_Id = @ClientId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return list;
        }

        public async Task<List<ClientRole>> GetByRoleName(string roleName)
        {
            var list = new List<ClientRole>();
            string selectQuery = "SELECT Role_Name, Client_Id FROM Client_Role WHERE Role_Name = @RoleName";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return list;
        }

        public async Task Delete(string roleName, int clientId)
        {
            string deleteQuery = "DELETE FROM Client_Role WHERE Role_Name = @RoleName AND Client_Id = @ClientId";
            OpenConnection();

            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@RoleName", roleName);
                command.Parameters.AddWithValue("@ClientId", clientId);
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }
    }
}

