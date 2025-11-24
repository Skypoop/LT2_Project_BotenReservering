using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientManagementTaskRepository : DatabaseConnection, IClientManagementTaskRepository
    {
        public ClientManagementTaskRepository()
        {
        }

        public static async Task<ClientManagementTaskRepository> CreateAsync()
        {
            ClientManagementTaskRepository repo = new ClientManagementTaskRepository();

            await repo.CreateTableAsync(@"CREATE TABLE IF NOT EXISTS Client_ManagementTask (
                            [Client_Id] INT NOT NULL,
                            [Management_Task_Id] INT NOT NULL,
                            PRIMARY KEY (Client_Id, Management_Task_Id),
                            FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                            FOREIGN KEY (Management_Task_Id) REFERENCES ManagementTask(Id))");

            return repo;
        }

        public async Task<ClientManagementTask> Add(ClientManagementTask item)
        {
            string insertQuery = @"INSERT INTO Client_ManagementTask(Client_Id, Management_Task_Id) 
                                   VALUES(@ClientId, @ManagementTaskId)";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ClientId", item.ClientId);
                    command.Parameters.AddWithValue("@ManagementTaskId", item.ManagementTaskId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }

            return item;
        }

        public async Task<List<ClientManagementTask>> GetByClientId(int clientId)
        {
            List<ClientManagementTask> list = new List<ClientManagementTask>();
            string selectQuery = "SELECT Client_Id, Management_Task_Id FROM Client_ManagementTask WHERE Client_Id = @ClientId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int clientIdFromDb = reader.GetInt32(0);
                            int managementTaskIdFromDb = reader.GetInt32(1);
                            ClientManagementTask clientManagementTask = new ClientManagementTask(clientIdFromDb, managementTaskIdFromDb);
                            list.Add(clientManagementTask);
                        }
                    }
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }

            return list;
        }

        public async Task<List<ClientManagementTask>> GetByManagementTaskId(int managementTaskId)
        {
            List<ClientManagementTask> list = new List<ClientManagementTask>();
            string selectQuery = "SELECT Client_Id, Management_Task_Id FROM Client_ManagementTask WHERE Management_Task_Id = @ManagementTaskId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ManagementTaskId", managementTaskId);

                    using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int clientIdFromDb = reader.GetInt32(0);
                            int managementTaskIdFromDb = reader.GetInt32(1);
                            ClientManagementTask clientManagementTask = new ClientManagementTask(clientIdFromDb, managementTaskIdFromDb);
                            list.Add(clientManagementTask);
                        }
                    }
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }

            return list;
        }

        public async Task Delete(int clientId, int managementTaskId)
        {
            string deleteQuery = "DELETE FROM Client_ManagementTask WHERE Client_Id = @ClientId AND Management_Task_Id = @ManagementTaskId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(deleteQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@ManagementTaskId", managementTaskId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }
    }
}
