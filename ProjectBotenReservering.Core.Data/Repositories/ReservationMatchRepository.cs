using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ReservationMatchRepository : DatabaseConnection, IReservationMatchRepository
    {
        public ReservationMatchRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Reservation_Match (
                            [Match_Id] INT NOT NULL,
                            [Reservation_Id] INT NOT NULL,
                            [Team_Name] VARCHAR NOT NULL,
                            PRIMARY KEY(Match_Id, Reservation_Id),
                            FOREIGN KEY(Match_Id) REFERENCES Match(Id),
                            FOREIGN KEY(Reservation_Id) REFERENCES Reservation(Id))");
        }

        public ReservationMatch Add(ReservationMatch item)
        {
            string insertQuery = @"INSERT INTO Reservation_Match(Match_Id, Reservation_Id, Team_Name)
                                   VALUES(@MatchId, @ReservationId, @TeamName)";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@MatchId", item.MatchId);
                command.Parameters.AddWithValue("@ReservationId", item.ReservationId);
                command.Parameters.AddWithValue("@TeamName", item.TeamName);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public ReservationMatch? Get(int matchId, int reservationId)
        {
            ReservationMatch? reservationMatch = null;
            string selectQuery = "SELECT Match_Id, Reservation_Id, Team_Name FROM Reservation_Match WHERE Match_Id = @MatchId AND Reservation_Id = @ReservationId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@MatchId", matchId);
                command.Parameters.AddWithValue("@ReservationId", reservationId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reservationMatch = MapReaderToReservationMatch(reader);
                    }
                }
            }

            CloseConnection();
            return reservationMatch;
        }

        public List<ReservationMatch> GetByMatchId(int matchId)
        {
            var list = new List<ReservationMatch>();
            string selectQuery = "SELECT Match_Id, Reservation_Id, Team_Name FROM Reservation_Match WHERE Match_Id = @MatchId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@MatchId", matchId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapReaderToReservationMatch(reader));
                    }
                }
            }

            CloseConnection();
            return list;
        }

        public List<ReservationMatch> GetByReservationId(int reservationId)
        {
            var list = new List<ReservationMatch>();
            string selectQuery = "SELECT Match_Id, Reservation_Id, Team_Name FROM Reservation_Match WHERE Reservation_Id = @ReservationId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@ReservationId", reservationId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapReaderToReservationMatch(reader));
                    }
                }
            }

            CloseConnection();
            return list;
        }

        public void Delete(int matchId, int reservationId)
        {
            string deleteQuery = "DELETE FROM Reservation_Match WHERE Match_Id = @MatchId AND Reservation_Id = @ReservationId";
            OpenConnection();
            using (SqliteCommand command = new(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@MatchId", matchId);
                command.Parameters.AddWithValue("@ReservationId", reservationId);
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        private ReservationMatch MapReaderToReservationMatch(SqliteDataReader reader)
        {
            return new ReservationMatch(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2)
            );
        }
    }
}

