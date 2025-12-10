using Microsoft.Data.Sqlite;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class RoleManagementTaskRepository : DatabaseConnection, IRoleManagementTaskRepository
    {
        public RoleManagementTaskRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Role_ManagementTask (
                            [Role_Id] VARCHAR(50) NOT NULL,
                            [ManagementTask_Id] INT NOT NULL,
                            PRIMARY KEY (Role_Id, ManagementTask_Id),
                            FOREIGN KEY (Role_Id) REFERENCES Role(Name),
                            FOREIGN KEY (ManagementTask_Id) REFERENCES ManagementTask(Id))");
        }

        public RoleManagementTask Add(RoleManagementTask item)
        {
            string insertQuery = @"INSERT INTO Role_ManagementTask(Role_Id, ManagementTask_Id) 
                                   VALUES(@RoleId, @ManagementTaskId)";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@RoleId", item.RoleId);
                command.Parameters.AddWithValue("@ManagementTaskId", item.ManagementTaskId);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public List<RoleManagementTask> GetByRoleId(string roleId)
        {
            var list = new List<RoleManagementTask>();
            string selectQuery = "SELECT Role_Id, ManagementTask_Id FROM Role_ManagementTask WHERE Role_Id = @RoleId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return list;
        }

        public List<RoleManagementTask> GetByManagementTaskId(int managementTaskId)
        {
            var list = new List<RoleManagementTask>();
            string selectQuery = "SELECT Role_Id, ManagementTask_Id FROM Role_ManagementTask WHERE ManagementTask_Id = @ManagementTaskId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return list;
        }

        public void Delete(string roleId, int managementTaskId)
        {
            string deleteQuery = "DELETE FROM Role_ManagementTask WHERE Role_Id = @RoleId AND ManagementTask_Id = @ManagementTaskId";
            OpenConnection();

            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@RoleId", roleId);
                command.Parameters.AddWithValue("@ManagementTaskId", managementTaskId);
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }
    }
}

