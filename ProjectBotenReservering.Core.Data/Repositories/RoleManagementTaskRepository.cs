using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class RoleManagementTaskRepository : IRoleManagementTaskRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<RoleManagementTask> _mapper;

        public RoleManagementTaskRepository(IDbConnectionFactory connectionFactory, IMapper<RoleManagementTask> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public RoleManagementTask Add(RoleManagementTask item)
        {
            string insertQuery = @"INSERT INTO Role_ManagementTask(Role_Id, ManagementTask_Id) 
                                   VALUES(@RoleId, @ManagementTaskId)";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@RoleId", item.RoleId);
                    command.AddParameter("@ManagementTaskId", item.ManagementTaskId);
                    command.ExecuteNonQuery();
                }
            }
            return item;
        }

        public List<RoleManagementTask> GetByRoleId(string roleId)
        {
            List<RoleManagementTask> list = new List<RoleManagementTask>();
            string selectQuery = "SELECT Role_Id, ManagementTask_Id FROM Role_ManagementTask WHERE Role_Id = @RoleId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@RoleId", roleId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return list;
        }

        public List<RoleManagementTask> GetByManagementTaskId(int managementTaskId)
        {
            List<RoleManagementTask> list = new List<RoleManagementTask>();
            string selectQuery = "SELECT Role_Id, ManagementTask_Id FROM Role_ManagementTask WHERE ManagementTask_Id = @ManagementTaskId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@ManagementTaskId", managementTaskId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return list;
        }

        public void Delete(string roleId, int managementTaskId)
        {
            string deleteQuery = "DELETE FROM Role_ManagementTask WHERE Role_Id = @RoleId AND ManagementTask_Id = @ManagementTaskId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.AddParameter("@RoleId", roleId);
                    command.AddParameter("@ManagementTaskId", managementTaskId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}