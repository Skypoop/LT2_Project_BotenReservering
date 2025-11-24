using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ReservationRepository : DatabaseConnection, IReservationRepository
    {
        public ReservationRepository()
        {
        }

        public static async Task<ReservationRepository> CreateAsync()
        {
            ReservationRepository repo = new ReservationRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS Reservation (
                    [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    [Created_At] DATETIME NOT NULL,
                    [Start_Time] DATETIME NOT NULL,
                    [End_Time] DATETIME NOT NULL,
                    [Client_Id] INT NOT NULL,
                    [Boat_Id] INT NOT NULL,
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                    FOREIGN KEY (Boat_Id) REFERENCES Boat(Id))");

            return repo;
        }

        public async Task<Reservation> Add(Reservation item)
        {
            string insertQuery = @"INSERT INTO Reservation(Created_At, Start_Time, End_Time, Client_Id, Boat_Id) 
                                   VALUES(@CreatedAt, @StartTime, @EndTime, @ClientId, @BoatId);
                                   SELECT last_insert_rowid();";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
                    command.Parameters.AddWithValue("@StartTime", item.StartTime);
                    command.Parameters.AddWithValue("@EndTime", item.EndTime);
                    command.Parameters.AddWithValue("@ClientId", item.ClientId);
                    command.Parameters.AddWithValue("@BoatId", item.BoatId);

                    object? result = command.ExecuteScalar();
                    if (result != null)
                    {
                        item.Id = Convert.ToInt32(result);
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return item;
        }

        public async Task<Reservation?> Get(int id)
        {
            Reservation? reservation = null;
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id FROM Reservation WHERE Id = @Id";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
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
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return reservation;
        }

        public async Task<List<Reservation>> GetAll()
        {
            List<Reservation> reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id FROM Reservation";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservationList.Add(MapReaderToReservation(reader));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return reservationList;
        }

        public async Task<List<Reservation>> GetByClientId(int clientId)
        {
            List<Reservation> reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id FROM Reservation WHERE Client_Id = @ClientId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
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
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return reservationList;
        }

        public async Task<List<Reservation>> GetByBoatId(int boatId)
        {
            List<Reservation> reservationList = new List<Reservation>();
            string selectQuery = "SELECT Id, Created_At, Start_Time, End_Time, Client_Id, Boat_Id FROM Reservation WHERE Boat_Id = @BoatId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
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
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return reservationList;
        }

        private Reservation MapReaderToReservation(SqliteDataReader reader)
        {
            return new Reservation(
                reader.GetInt32(0),
                reader.GetDateTime(1),
                reader.GetDateTime(2),
                reader.GetDateTime(3),
                reader.GetInt32(4),
                reader.GetInt32(5)
            );
        }
    }
}
