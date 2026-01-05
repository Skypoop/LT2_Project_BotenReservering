using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ReservationCompetitionRepository : IReservationCompetitionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<ReservationCompetition> _mapper;

        public ReservationCompetitionRepository(IDbConnectionFactory connectionFactory, IMapper<ReservationCompetition> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public ReservationCompetition Add(ReservationCompetition item)
        {
            string insertQuery = @"INSERT INTO Reservation_Competition(Competition_Id, Reservation_Id, Team_Name)
                                   VALUES(@CompetitionId, @ReservationId, @TeamName)";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@CompetitionId", item.CompetitionId);
                    command.AddParameter("@ReservationId", item.ReservationId);
                    command.AddParameter("@TeamName", item.TeamName);
                    command.ExecuteNonQuery();
                }
            }
            return item;
        }

        public ReservationCompetition? Get(int competitionId, int reservationId)
        {
            ReservationCompetition? reservationCompetition = null;
            string selectQuery = "SELECT Competition_Id, Reservation_Id, Team_Name FROM Reservation_Competition WHERE Competition_Id = @CompetitionId AND Reservation_Id = @ReservationId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@CompetitionId", competitionId);
                    command.AddParameter("@ReservationId", reservationId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reservationCompetition = _mapper.Map(reader);
                        }
                    }
                }
            }
            return reservationCompetition;
        }

        public List<ReservationCompetition> GetByCompetitionId(int competitionId)
        {
            List<ReservationCompetition> list = new List<ReservationCompetition>();
            string selectQuery = "SELECT Competition_Id, Reservation_Id, Team_Name FROM Reservation_Competition WHERE Competition_Id = @CompetitionId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = selectQuery;
                    command.AddParameter("@CompetitionId", competitionId);
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

        public List<ReservationCompetition> GetByReservationId(int reservationId)
        {
            List<ReservationCompetition> list = new List<ReservationCompetition>();
            string selectQuery = "SELECT Competition_Id, Reservation_Id, Team_Name FROM Reservation_Competition WHERE Reservation_Id = @ReservationId";

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

        public void Delete(int competitionId, int reservationId)
        {
            string deleteQuery = "DELETE FROM Reservation_Competition WHERE Competition_Id = @CompetitionId AND Reservation_Id = @ReservationId";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.AddParameter("@CompetitionId", competitionId);
                    command.AddParameter("@ReservationId", reservationId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}