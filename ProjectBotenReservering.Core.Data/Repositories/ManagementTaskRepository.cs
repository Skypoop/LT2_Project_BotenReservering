using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ManagementTaskRepository : DatabaseConnection, IManagementTaskRepository
    {
        public ManagementTaskRepository()
        {
        }

        public static async Task<ManagementTaskRepository> CreateAsync()
        {
            ManagementTaskRepository repo = new ManagementTaskRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS ManagementTask (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Name] VARCHAR(50) NOT NULL)");

            return repo;
        }

        public async Task<ManagementTask> Add(ManagementTask item)
        {
            string insertQuery = @"INSERT INTO ManagementTask(Name) VALUES(@Name);
                                   SELECT last_insert_rowid();";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Name", item.Name);
                    object? result = command.ExecuteScalar();
                    if (result != null)
                    {
                        item.Id = Convert.ToInt32(result);
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return item;
        }

        public async Task<ManagementTask?> Get(int id)
        {
            ManagementTask? task = null;
            string selectQuery = "SELECT Id, Name FROM ManagementTask WHERE Id = @Id";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            task = new ManagementTask(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return task;
        }

        public async Task<List<ManagementTask>> GetAll()
        {
            List<ManagementTask> taskList = new List<ManagementTask>();
            string selectQuery = "SELECT Id, Name FROM ManagementTask";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            taskList.Add(new ManagementTask(reader.GetInt32(0), reader.GetString(1)));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return taskList;
        }
    }
}
