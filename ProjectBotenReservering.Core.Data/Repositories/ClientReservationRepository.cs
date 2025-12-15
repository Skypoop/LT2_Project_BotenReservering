using System.Data;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;    
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientReservationRepository : IClientReservationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<ClientReservation> _mapper;

        public ClientReservationRepository(IDbConnectionFactory connectionFactory, IMapper<ClientReservation> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public ClientReservation Add(ClientReservation item)
        {
            string insertQuery = @"INSERT INTO Client_Reservation(Client_Id, Reservation_Id) 
                                   VALUES(@ClientId, @ReservationId)";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@ClientId", item.ClientId);
                    command.AddParameter("@ReservationId", item.ReservationId);
                    command.ExecuteNonQuery();
                }
            }
            return item;
        }

        public List<ClientReservation> GetByClientId(int clientId)
        {
            List<ClientReservation> list = new List<ClientReservation>();
            string selectQuery = "SELECT Client_Id, Reservation_Id FROM Client_Reservation WHERE Client_Id = @ClientId";

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

        public List<ClientReservation> GetByReservationId(int reservationId)
        {
            List<ClientReservation> list = new List<ClientReservation>();
            string selectQuery = "SELECT Client_Id, Reservation_Id FROM Client_Reservation WHERE Reservation_Id = @ReservationId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@ReservationId", reservationId);
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

        public ClientReservation? Get(int clientId, int reservationId)
        {
            ClientReservation? clientReservation = null;
            string selectQuery = "SELECT Client_Id, Reservation_Id FROM Client_Reservation WHERE Client_Id = @ClientId AND Reservation_Id = @ReservationId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@ClientId", clientId);
                    command.AddParameter("@ReservationId", reservationId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            clientReservation = _mapper.Map(reader);
                        }
                    }
                }
            }
            return clientReservation;
        }
    }
}