using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ManagementTaskRepository : IManagementTaskRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<ManagementTask> _mapper;

        public ManagementTaskRepository(IDbConnectionFactory connectionFactory, IMapper<ManagementTask> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public ManagementTask Add(ManagementTask item)
        {
            string insertQuery = @"INSERT INTO ManagementTask(Name) VALUES(@Name);
                                   SELECT last_insert_rowid();";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@Name", item.Name);
                    item.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return item;
        }

        public ManagementTask? Get(int id)
        {
            ManagementTask? task = null;
            string selectQuery = "SELECT Id, Name FROM ManagementTask WHERE Id = @Id";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@Id", id);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            task = _mapper.Map(reader);
                        }
                    }
                }
            }
            return task;
        }

        public List<ManagementTask> GetAll()
        {
            List<ManagementTask> taskList = new List<ManagementTask>();
            string selectQuery = "SELECT Id, Name FROM ManagementTask";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            taskList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return taskList;
        }
    }
}