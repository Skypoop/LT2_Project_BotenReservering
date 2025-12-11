using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ReservationMatchRepository : IReservationMatchRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<ReservationMatch> _mapper;

        public ReservationMatchRepository(IDbConnectionFactory connectionFactory, IMapper<ReservationMatch> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public ReservationMatch Add(ReservationMatch item)
        {
            string insertQuery = @"INSERT INTO Reservation_Match(Match_Id, Reservation_Id, Team_Name)
                                   VALUES(@MatchId, @ReservationId, @TeamName)";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@MatchId", item.MatchId);
                    command.AddParameter("@ReservationId", item.ReservationId);
                    command.AddParameter("@TeamName", item.TeamName);
                    command.ExecuteNonQuery();
                }
            }
            return item;
        }

        public ReservationMatch? Get(int matchId, int reservationId)
        {
            ReservationMatch? reservationMatch = null;
            string selectQuery = "SELECT Match_Id, Reservation_Id, Team_Name FROM Reservation_Match WHERE Match_Id = @MatchId AND Reservation_Id = @ReservationId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@MatchId", matchId);
                    command.AddParameter("@ReservationId", reservationId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reservationMatch = _mapper.Map(reader);
                        }
                    }
                }
            }
            return reservationMatch;
        }

        public List<ReservationMatch> GetByMatchId(int matchId)
        {
            List<ReservationMatch> list = new List<ReservationMatch>();
            string selectQuery = "SELECT Match_Id, Reservation_Id, Team_Name FROM Reservation_Match WHERE Match_Id = @MatchId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@MatchId", matchId);
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

        public List<ReservationMatch> GetByReservationId(int reservationId)
        {
            List<ReservationMatch> list = new List<ReservationMatch>();
            string selectQuery = "SELECT Match_Id, Reservation_Id, Team_Name FROM Reservation_Match WHERE Reservation_Id = @ReservationId";

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

        public void Delete(int matchId, int reservationId)
        {
            string deleteQuery = "DELETE FROM Reservation_Match WHERE Match_Id = @MatchId AND Reservation_Id = @ReservationId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.AddParameter("@MatchId", matchId);
                    command.AddParameter("@ReservationId", reservationId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}