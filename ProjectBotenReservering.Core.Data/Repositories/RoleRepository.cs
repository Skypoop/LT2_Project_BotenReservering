using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<Role> _mapper;

        public RoleRepository(IDbConnectionFactory connectionFactory, IMapper<Role> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public Role Add(Role item)
        {
            string insertQuery = @"INSERT INTO Role(Name) VALUES(@Name)";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@Name", item.Name);
                    command.ExecuteNonQuery();
                }
            }
            return item;
        }

        public Role? Get(string name)
        {
            Role? role = null;
            string selectQuery = "SELECT Name FROM Role WHERE Name = @Name";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@Name", name);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role = _mapper.Map(reader);
                        }
                    }
                }
            }
            return role;
        }

        public List<Role> GetAll()
        {
            List<Role> roleList = new List<Role>();
            string selectQuery = "SELECT Name FROM Role";

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
                            roleList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return roleList;
        }

    }
}