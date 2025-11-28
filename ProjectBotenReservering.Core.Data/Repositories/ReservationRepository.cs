using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ReservationRepository : DatabaseConnection, IReservationRepository
    {
        public ReservationRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Reservation (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Created_At] DATETIME NOT NULL,
                            [Start_Time] DATETIME NOT NULL,
                            [End_Time] DATETIME NOT NULL,
                            [Client_Id] INT NOT NULL,
                            [Boat_Id] INT NOT NULL,
                            [Approved] BOOLEAN NOT NULL,
                            FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                            FOREIGN KEY (Boat_Id) REFERENCES Boat(Id))");
        }

        public Reservation Add(Reservation item)
        {
            string insertQuery = @"INSERT INTO Reservation(Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved) 
                                   VALUES(@CreatedAt, @StartTime, @EndTime, @ClientId, @BoatId, @Approved);
                                   SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
                command.Parameters.AddWithValue("@StartTime", item.StartTime);
                command.Parameters.AddWithValue("@EndTime", item.EndTime);
                command.Parameters.AddWithValue("@ClientId", item.ClientId);
                command.Parameters.AddWithValue("@BoatId", item.BoatId);
                command.Parameters.AddWithValue("@Approved", item.Approved);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public Reservation? Get(int id)
        {
            Reservation? reservation = null;
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved FROM Reservation WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reservation = MapReaderToReservation(reader);
                    }
                }
            }

            CloseConnection();
            return reservation;
        }

        public List<Reservation> GetAll()
        {
            var reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved FROM Reservation";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservationList.Add(MapReaderToReservation(reader));
                    }
                }
            }

            CloseConnection();
            return reservationList;
        }

        public List<Reservation> GetByClientId(int clientId)
        {
            var reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved FROM Reservation WHERE Client_Id = @ClientId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservationList.Add(MapReaderToReservation(reader));
                    }
                }
            }

            CloseConnection();
            return reservationList;
        }

        public List<Reservation> GetByBoatId(int boatId)
        {
            var reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id FROM Reservation WHERE Boat_Id = @BoatId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@BoatId", boatId);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reservationList.Add(MapReaderToReservation(reader));
                    }
                }
            }

            CloseConnection();
            return reservationList;
        }

        private Reservation MapReaderToReservation(SqliteDataReader reader)
        {
            return new Reservation(
                reader.GetDateTime(1),
                reader.GetDateTime(2),
                reader.GetDateTime(3),
                reader.GetInt32(4),
                reader.GetInt32(5),
                reader.GetBoolean(6),
                reader.GetInt32(0)
            );
        }
    }
}

