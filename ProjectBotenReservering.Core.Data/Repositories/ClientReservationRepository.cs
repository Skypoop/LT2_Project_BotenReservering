using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientReservationRepository : DatabaseConnection, IClientReservationRepository
    {
        public ClientReservationRepository()
        {
        }

        public static async Task<ClientReservationRepository> CreateAsync()
        {
            ClientReservationRepository repo = new ClientReservationRepository();

            await repo.CreateTableAsync(@"
                CREATE TABLE IF NOT EXISTS Client_Reservation (
                    [Client_Id] INT NOT NULL,
                    [Reservation_Id] INT NOT NULL,
                    [Approved] BOOLEAN NOT NULL,
                    PRIMARY KEY (Client_Id, Reservation_Id),
                    FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                    FOREIGN KEY (Reservation_Id) REFERENCES Reservation(Id))");

            return repo;
        }

        public async Task<ClientReservation> Add(ClientReservation item)
        {
            string insertQuery = @"INSERT INTO Client_Reservation(Client_Id, Reservation_Id, Approved) 
                                   VALUES(@ClientId, @ReservationId, @Approved)";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(insertQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ClientId", item.ClientId);
                    command.Parameters.AddWithValue("@ReservationId", item.ReservationId);
                    command.Parameters.AddWithValue("@Approved", item.Approved);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return item;
        }

        public async Task<List<ClientReservation>> GetByClientId(int clientId)
        {
            List<ClientReservation> list = new List<ClientReservation>();
            string selectQuery = "SELECT Client_Id, Reservation_Id, Approved FROM Client_Reservation WHERE Client_Id = @ClientId";

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
                            list.Add(new ClientReservation(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetBoolean(2)
                            ));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return list;
        }

        public async Task<List<ClientReservation>> GetByReservationId(int reservationId)
        {
            List<ClientReservation> list = new List<ClientReservation>();
            string selectQuery = "SELECT Client_Id, Reservation_Id, Approved FROM Client_Reservation WHERE Reservation_Id = @ReservationId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ReservationId", reservationId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ClientReservation(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetBoolean(2)
                            ));
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return list;
        }

        public async Task<ClientReservation?> Get(int clientId, int reservationId)
        {
            ClientReservation? clientReservation = null;
            string selectQuery = "SELECT Client_Id, Reservation_Id, Approved FROM Client_Reservation WHERE Client_Id = @ClientId AND Reservation_Id = @ReservationId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(selectQuery, Connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@ReservationId", reservationId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            clientReservation = new ClientReservation(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetBoolean(2)
                            );
                        }
                    }
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }

            return clientReservation;
        }

        public async Task UpdateApproval(int clientId, int reservationId, bool approved)
        {
            string updateQuery = "UPDATE Client_Reservation SET Approved = @Approved WHERE Client_Id = @ClientId AND Reservation_Id = @ReservationId";

            await OpenConnectionAsync();

            try
            {
                using (SqliteCommand command = new SqliteCommand(updateQuery, Connection))
                {
                    command.Parameters.AddWithValue("@Approved", approved);
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@ReservationId", reservationId);
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                _ = CloseConnectionAsync();
            }
        }
    }
}
