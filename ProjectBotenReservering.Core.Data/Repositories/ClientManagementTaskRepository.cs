using System.Data;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientManagementTaskRepository : IClientManagementTaskRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<ClientManagementTask> _mapper;

        public ClientManagementTaskRepository(IDbConnectionFactory connectionFactory, IMapper<ClientManagementTask> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public ClientManagementTask Add(ClientManagementTask item)
        {
            string insertQuery = @"INSERT INTO Client_ManagementTask(Client_Id, Management_Task_Id) 
                                   VALUES(@ClientId, @ManagementTaskId)";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@ClientId", item.ClientId);
                    command.AddParameter("@ManagementTaskId", item.ManagementTaskId);
                    command.ExecuteNonQuery();
                }
            }
            return item;
        }

        public List<ClientManagementTask> GetByClientId(int clientId)
        {
            List<ClientManagementTask> list = new List<ClientManagementTask>();
            string selectQuery = "SELECT Client_Id, Management_Task_Id FROM Client_ManagementTask WHERE Client_Id = @ClientId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@ClientId", clientId);
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

        public List<ClientManagementTask> GetByManagementTaskId(int managementTaskId)
        {
            List<ClientManagementTask> list = new List<ClientManagementTask>();
            string selectQuery = "SELECT Client_Id, Management_Task_Id FROM Client_ManagementTask WHERE Management_Task_Id = @ManagementTaskId";

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

        public void Delete(int clientId, int managementTaskId)
        {
            string deleteQuery = "DELETE FROM Client_ManagementTask WHERE Client_Id = @ClientId AND Management_Task_Id = @ManagementTaskId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.AddParameter("@ClientId", clientId);
                    command.AddParameter("@ManagementTaskId", managementTaskId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}