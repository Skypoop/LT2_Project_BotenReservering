using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class RoleManagementTaskRepository : DatabaseConnection, IRoleManagementTaskRepository
    {
        public RoleManagementTaskRepository()
        {
        }

        public static async Task<RoleManagementTaskRepository> CreateAsync()
        {
            RoleManagementTaskRepository repo = new RoleManagementTaskRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS Role_ManagementTask (
                    [Role_Id] VARCHAR(50) NOT NULL,
                    [ManagementTask_Id] INT NOT NULL,
                    PRIMARY KEY (Role_Id, ManagementTask_Id),
                    FOREIGN KEY (Role_Id) REFERENCES Role(Name),
                    FOREIGN KEY (ManagementTask_Id) REFERENCES ManagementTask(Id))");

            return repo;
        }

        public async Task<RoleManagementTask> Add(RoleManagementTask item)
        {
            string insertQuery = @"INSERT INTO Role_ManagementTask(Role_Id, ManagementTask_Id) 
                                   VALUES(@RoleId, @ManagementTaskId)";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@RoleId", item.RoleId);
                    command.Parameters.AddWithValue("@ManagementTaskId", item.ManagementTaskId);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return item;
        }

        public async Task<List<RoleManagementTask>> GetByRoleId(string roleId)
        {
            List<RoleManagementTask> list = new List<RoleManagementTask>();
            string selectQuery = "SELECT Role_Id, ManagementTask_Id FROM Role_ManagementTask WHERE Role_Id = @RoleId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@RoleId", roleId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RoleManagementTask(reader.GetString(0), reader.GetInt32(1)));
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

        public async Task<List<RoleManagementTask>> GetByManagementTaskId(int managementTaskId)
        {
            List<RoleManagementTask> list = new List<RoleManagementTask>();
            string selectQuery = "SELECT Role_Id, ManagementTask_Id FROM Role_ManagementTask WHERE ManagementTask_Id = @ManagementTaskId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ManagementTaskId", managementTaskId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RoleManagementTask(reader.GetString(0), reader.GetInt32(1)));
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

        public async Task Delete(string roleId, int managementTaskId)
        {
            string deleteQuery = "DELETE FROM Role_ManagementTask WHERE Role_Id = @RoleId AND ManagementTask_Id = @ManagementTaskId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(deleteQuery, Connection))
                {
                    command.Parameters.AddWithValue("@RoleId", roleId);
                    command.Parameters.AddWithValue("@ManagementTaskId", managementTaskId);
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
