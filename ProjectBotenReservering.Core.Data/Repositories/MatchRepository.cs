using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class MatchRepository : DatabaseConnection, IMatchRepository
    {
        public MatchRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Match (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Start_DateTime] DATETIME NOT NULL,
                            [End_DateTime] DATETIME NOT NULL,
                            [Match_Name] VARCHAR NOT NULL)");
        }

        public Match Add(Match item)
        {
            string insertQuery = @"INSERT INTO Match(Start_DateTime, End_DateTime, Match_Name)
                                   VALUES(@StartDateTime, @EndDateTime, @MatchName);
                                   SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@StartDateTime", item.StartDateTime);
                command.Parameters.AddWithValue("@EndDateTime", item.EndDateTime);
                command.Parameters.AddWithValue("@MatchName", item.MatchName);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }

            CloseConnection();
            return item;
        }

        public Match? Get(int id)
        {
            Match? match = null;
            string selectQuery = "SELECT Id, Start_DateTime, End_DateTime, Match_Name FROM Match WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        match = MapReaderToMatch(reader);
                    }
                }
            }

            CloseConnection();
            return match;
        }

        public List<Match> GetAll()
        {
            var matchList = new List<Match>();
            string selectQuery = "SELECT Id, Start_DateTime, End_DateTime, Match_Name FROM Match";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matchList.Add(MapReaderToMatch(reader));
                    }
                }
            }

            CloseConnection();
            return matchList;
        }

        public void Delete(int id)
        {
            string deleteQuery = "DELETE FROM Match WHERE Id = @Id";
            OpenConnection();
            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        private Match MapReaderToMatch(SqliteDataReader reader)
        {
            return new Match
            {
                Id = reader.GetInt32(0),
                StartDateTime = reader.GetDateTime(1),
                EndDateTime = reader.GetDateTime(2),
                MatchName = reader.GetString(3)
            };
        }

        public Match SaveMatchWithReservations(Match match, List<int> reservationIds, List<string> teamNames)
        {
            OpenConnection();
            using (var transaction = Connection.BeginTransaction())
            {
                try
                {
                    // Insert match
                    string insertMatchQuery = @"INSERT INTO Match(Start_DateTime, End_DateTime, Match_Name)
                                               VALUES(@StartDateTime, @EndDateTime, @MatchName);
                                               SELECT last_insert_rowid();";

                    using (SqliteCommand command = new(insertMatchQuery, Connection, transaction))
                    {
                        command.Parameters.AddWithValue("@StartDateTime", match.StartDateTime);
                        command.Parameters.AddWithValue("@EndDateTime", match.EndDateTime);
                        command.Parameters.AddWithValue("@MatchName", match.MatchName);
                        match.Id = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Link existing reservations to the match
                    string insertLinkQuery = @"INSERT INTO Reservation_Match(Match_Id, Reservation_Id, Team_Name)
                                              VALUES(@MatchId, @ReservationId, @TeamName)";

                    for (int i = 0; i < reservationIds.Count; i++)
                    {
                        using (SqliteCommand command = new(insertLinkQuery, Connection, transaction))
                        {
                            command.Parameters.AddWithValue("@MatchId", match.Id);
                            command.Parameters.AddWithValue("@ReservationId", reservationIds[i]);
                            command.Parameters.AddWithValue("@TeamName", teamNames[i]);
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

            CloseConnection();
            return match;
        }

        public void CancelReservationAndUpdateStatus(int reservationId, int matchId)
        {
            OpenConnection();
            using (var transaction = Connection.BeginTransaction())
            {
                try
                {
                    // Delete from Reservation_Match
                    string deleteLinkQuery =
                        "DELETE FROM Reservation_Match WHERE Match_Id = @MatchId AND Reservation_Id = @ReservationId";
                    using (SqliteCommand command = new(deleteLinkQuery, Connection, transaction))
                    {
                        command.Parameters.AddWithValue("@MatchId", matchId);
                        command.Parameters.AddWithValue("@ReservationId", reservationId);
                        command.ExecuteNonQuery();
                    }

                    // Update reservation status to cancelled (Approved = false)
                    string updateStatusQuery = "UPDATE Reservation SET Approved = 0 WHERE Id = @ReservationId";
                    using (SqliteCommand command = new(updateStatusQuery, Connection, transaction))
                    {
                        command.Parameters.AddWithValue("@ReservationId", reservationId);
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

            CloseConnection();
        }
    }
}