using System.Data;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Data.Helpers;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientRoleRepository : IClientRoleRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<ClientRole> _mapper;

        public ClientRoleRepository(IDbConnectionFactory connectionFactory, IMapper<ClientRole> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public ClientRole Add(ClientRole item)
        {
            string insertQuery = @"INSERT INTO Client_Role(Role_Name, Client_Id) 
                                   VALUES(@RoleName, @ClientId)";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@RoleName", item.RoleName);
                    command.AddParameter("@ClientId", item.ClientId);
                    command.ExecuteNonQuery();
                }
            }
            return item;
        }

        public List<ClientRole> GetByClientId(int clientId)
        {
            List<ClientRole> list = new List<ClientRole>();
            string selectQuery = "SELECT Role_Name, Client_Id FROM Client_Role WHERE Client_Id = @ClientId";

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

        public List<ClientRole> GetByRoleName(string roleName)
        {
            List<ClientRole> list = new List<ClientRole>();
            string selectQuery = "SELECT Role_Name, Client_Id FROM Client_Role WHERE Role_Name = @RoleName";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@RoleName", roleName);
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

        public void Delete(string roleName, int clientId)
        {
            string deleteQuery = "DELETE FROM Client_Role WHERE Role_Name = @RoleName AND Client_Id = @ClientId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.AddParameter("@RoleName", roleName);
                    command.AddParameter("@ClientId", clientId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}