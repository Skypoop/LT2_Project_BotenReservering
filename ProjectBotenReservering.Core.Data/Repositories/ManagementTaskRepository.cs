using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ManagementTaskRepository : DatabaseConnection, IManagementTaskRepository
    {
        public ManagementTaskRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS ManagementTask (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Name] VARCHAR(50) NOT NULL)");
        }

        public ManagementTask Add(ManagementTask item)
        {
            string insertQuery = @"INSERT INTO ManagementTask(Name) VALUES(@Name);
                                   SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public ManagementTask? Get(int id)
        {
            ManagementTask? task = null;
            string selectQuery = "SELECT Id, Name FROM ManagementTask WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return task;
        }

        public List<ManagementTask> GetAll()
        {
            var taskList = new List<ManagementTask>();
            string selectQuery = "SELECT Id, Name FROM ManagementTask";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        taskList.Add(new ManagementTask(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }

            CloseConnection();
            return taskList;
        }
    }
}

