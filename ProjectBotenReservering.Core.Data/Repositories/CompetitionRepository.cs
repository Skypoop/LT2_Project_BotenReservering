using System.Data;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Interfaces.Mappers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class CompetitionRepository : ICompetitionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IMapper<Competition> _mapper;

        public CompetitionRepository(IDbConnectionFactory connectionFactory, IMapper<Competition> mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public Competition Add(Competition item)
        {
            string insertQuery = @"INSERT INTO Competition(Start_DateTime, End_DateTime, Competition_Name)
                                   VALUES(@StartDateTime, @EndDateTime, @CompetitionName);
                                   SELECT last_insert_rowid();";

            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = insertQuery;
                    command.AddParameter("@StartDateTime", item.StartDateTime);
                    command.AddParameter("@EndDateTime", item.EndDateTime);
                    command.AddParameter("@CompetitionName", (object?)item.CompetitionName ?? DBNull.Value);

                    item.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return item;
        }

        public Competition? Get(int id)
        {
            Competition? match = null;
            string selectQuery = "SELECT Id, Start_DateTime, End_DateTime, Competition_Name FROM Competition WHERE Id = @Id";

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

        public List<Competition> GetAll()
        {
            List<Competition> matchList = new List<Competition>();
            string selectQuery = "SELECT Id, Start_DateTime, End_DateTime, Competition_Name FROM Competition";

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
            string deleteQuery = "DELETE FROM Competition WHERE Id = @Id";

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

        public Competition SaveCompetitionWithReservations(Competition competition, List<int> reservationIds, List<string> teamNames)
        {
            using (IDbConnection connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertCompetitionQuery = @"INSERT INTO Competition(Start_DateTime, End_DateTime, Competition_Name)
                                                   VALUES(@StartDateTime, @EndDateTime, @CompetitionName);
                                                   SELECT last_insert_rowid();";

                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = insertCompetitionQuery;
                            command.AddParameter("@StartDateTime", competition.StartDateTime);
                            command.AddParameter("@EndDateTime", competition.EndDateTime);
                            command.AddParameter("@CompetitionName", (object?)competition.CompetitionName ?? DBNull.Value);
                            competition.Id = Convert.ToInt32(command.ExecuteScalar());
                        }

                        string insertLinkQuery = @"INSERT INTO Reservation_Competition(Competition_Id, Reservation_Id, Team_Name)
                                                  VALUES(@CompetitionId, @ReservationId, @TeamName)";

                        for (int i = 0; i < reservationIds.Count; i++)
                        {
                            using (IDbCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = insertLinkQuery;
                                command.AddParameter("@CompetitionId", competition.Id);
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
            return competition;
        }
    }
}