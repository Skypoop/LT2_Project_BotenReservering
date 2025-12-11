using System.Data;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<Match> _mapper;

        public MatchRepository(IDbConnectionFactory connectionFactory, IMapper<Match> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public Match Add(Match item)
        {
            string insertQuery = @"INSERT INTO Match(Start_DateTime, End_DateTime, Match_Name)
                                   VALUES(@StartDateTime, @EndDateTime, @MatchName);
                                   SELECT last_insert_rowid();";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@StartDateTime", item.StartDateTime);
                    command.AddParameter("@EndDateTime", item.EndDateTime);
                    command.AddParameter("@MatchName", (object?)item.MatchName ?? DBNull.Value);

                    item.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return item;
        }

        public Match? Get(int id)
        {
            Match? match = null;
            string selectQuery = "SELECT Id, Start_DateTime, End_DateTime, Match_Name FROM Match WHERE Id = @Id";

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
                            match = _mapper.Map(reader);
                        }
                    }
                }
            }
            return match;
        }

        public List<Match> GetAll()
        {
            List<Match> matchList = new List<Match>();
            string selectQuery = "SELECT Id, Start_DateTime, End_DateTime, Match_Name FROM Match";

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
                            matchList.Add(_mapper.Map(reader));
                        }
                    }
                }
            }
            return matchList;
        }

        public void Delete(int id)
        {
            string deleteQuery = "DELETE FROM Match WHERE Id = @Id";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteQuery;
                    command.AddParameter("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Match SaveMatchWithReservations(Match match, List<int> reservationIds, List<string> teamNames)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertMatchQuery = @"INSERT INTO Match(Start_DateTime, End_DateTime, Match_Name)
                                                   VALUES(@StartDateTime, @EndDateTime, @MatchName);
                                                   SELECT last_insert_rowid();";

                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = insertMatchQuery;
                            command.AddParameter("@StartDateTime", match.StartDateTime);
                            command.AddParameter("@EndDateTime", match.EndDateTime);
                            command.AddParameter("@MatchName", (object?)match.MatchName ?? DBNull.Value);
                            match.Id = Convert.ToInt32(command.ExecuteScalar());
                        }

                        string insertLinkQuery = @"INSERT INTO Reservation_Match(Match_Id, Reservation_Id, Team_Name)
                                                  VALUES(@MatchId, @ReservationId, @TeamName)";

                        for (int i = 0; i < reservationIds.Count; i++)
                        {
                            using (IDbCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = insertLinkQuery;
                                command.AddParameter("@MatchId", match.Id);
                                command.AddParameter("@ReservationId", reservationIds[i]);
                                command.AddParameter("@TeamName", (object?)teamNames[i] ?? DBNull.Value);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return match;
        }

        public void CancelReservationAndUpdateStatus(int reservationId, int matchId)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteLinkQuery =
                            "DELETE FROM Reservation_Match WHERE Match_Id = @MatchId AND Reservation_Id = @ReservationId";
                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = deleteLinkQuery;
                            command.AddParameter("@MatchId", matchId);
                            command.AddParameter("@ReservationId", reservationId);
                            command.ExecuteNonQuery();
                        }

                        string updateStatusQuery = "UPDATE Reservation SET Active = 0 WHERE Id = @ReservationId";
                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = updateStatusQuery;
                            command.AddParameter("@ReservationId", reservationId);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}