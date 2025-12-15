using System.Data;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<Client> _mapper;

        public ClientRepository(IDbConnectionFactory connectionFactory, IMapper<Client> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public Client Add(Client item)
        {
            string insertQuery = @"INSERT INTO Client(Full_Name, Email, Scull_level, Sweep_level, Club, Approved, Password_Hash) 
                                   VALUES(@FullName, @Email, @ScullLevel, @SweepLevel, @Club, @Approved, @PasswordHash);
                                   SELECT last_insert_rowid();";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@FullName", item.FullName);
                    command.AddParameter("@Email", item.Email);
                    command.AddParameter("@ScullLevel", item.ScullLevel);
                    command.AddParameter("@SweepLevel", item.SweepLevel);
                    command.AddParameter("@Club", item.Club ?? (object)DBNull.Value);
                    command.AddParameter("@Approved", item.Approved);
                    command.AddParameter("@PasswordHash", item.PasswordHash);

                    item.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return item;
        }

        public Client? Get(string email)
        {
            Client? client = null;
            string selectQuery = "SELECT Id, Full_Name, Email, Scull_level, Sweep_level, Club, Approved, Password_Hash FROM Client WHERE Email = @Email";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@Email", email);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            client = _mapper.Map(reader);
                        }
                    }
                }
            }
            return client;
        }

        public Client? Get(int id)
        {
            Client? client = null;
            string selectQuery = "SELECT Id, Full_Name, Email, Scull_level, Sweep_level, Club, Approved, Password_Hash FROM Client WHERE Id = @Id";

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
                            client = _mapper.Map(reader);
                        }
                    }
                }
            }
            return client;
        }

        public List<Client> GetAll()
        {
            List<Client> clientList = new List<Client>();
            string selectQuery = "SELECT * FROM Client";

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
                            clientList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return clientList;
        }
    }
}