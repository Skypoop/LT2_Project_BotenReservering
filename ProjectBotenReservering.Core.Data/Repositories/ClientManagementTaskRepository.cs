using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientManagementTaskRepository : DatabaseConnection, IClientManagementTaskRepository
    {
        public ClientManagementTaskRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Client_ManagementTask (
                            [Client_Id] INT NOT NULL,
                            [Management_Task_Id] INT NOT NULL,
                            PRIMARY KEY (Client_Id, Management_Task_Id),
                            FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                            FOREIGN KEY (Management_Task_Id) REFERENCES ManagementTask(Id))");
        }

        public async Task<ClientManagementTask> Add(ClientManagementTask item)
        {
            string insertQuery = @"INSERT INTO Client_ManagementTask(Client_Id, Management_Task_Id) 
                                   VALUES(@ClientId, @ManagementTaskId)";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@ClientId", item.ClientId);
                command.Parameters.AddWithValue("@ManagementTaskId", item.ManagementTaskId);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public async Task<List<ClientManagementTask>> GetByClientId(int clientId)
        {
            var list = new List<ClientManagementTask>();
            string selectQuery = "SELECT Client_Id, Management_Task_Id FROM Client_ManagementTask WHERE Client_Id = @ClientId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ClientManagementTask(reader.GetInt32(0), reader.GetInt32(1)));
                    }
                }
            }

            CloseConnection();
            return list;
        }

        public async Task<List<ClientManagementTask>> GetByManagementTaskId(int managementTaskId)
        {
            var list = new List<ClientManagementTask>();
            string selectQuery = "SELECT Client_Id, Management_Task_Id FROM Client_ManagementTask WHERE Management_Task_Id = @ManagementTaskId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@ManagementTaskId", managementTaskId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ClientManagementTask(reader.GetInt32(0), reader.GetInt32(1)));
                    }
                }
            }

            CloseConnection();
            return list;
        }

        public async Task Delete(int clientId, int managementTaskId)
        {
            string deleteQuery = "DELETE FROM Client_ManagementTask WHERE Client_Id = @ClientId AND Management_Task_Id = @ManagementTaskId";
            OpenConnection();

            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);
                command.Parameters.AddWithValue("@ManagementTaskId", managementTaskId);
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }
    }
}

