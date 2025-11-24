using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientReservationRepository : DatabaseConnection, IClientReservationRepository
    {
        public ClientReservationRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Client_Reservation (
                            [Client_Id] INT NOT NULL,
                            [Reservation_Id] INT NOT NULL,
                            [Approved] BOOLEAN NOT NULL,
                            PRIMARY KEY (Client_Id, Reservation_Id),
                            FOREIGN KEY (Client_Id) REFERENCES Client(Id),
                            FOREIGN KEY (Reservation_Id) REFERENCES Reservation(Id))");
        }

        public async Task<ClientReservation> Add(ClientReservation item)
        {
            string insertQuery = @"INSERT INTO Client_Reservation(Client_Id, Reservation_Id, Approved) 
                                   VALUES(@ClientId, @ReservationId, @Approved)";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@ClientId", item.ClientId);
                command.Parameters.AddWithValue("@ReservationId", item.ReservationId);
                command.Parameters.AddWithValue("@Approved", item.Approved);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public async Task<List<ClientReservation>> GetByClientId(int clientId)
        {
            var list = new List<ClientReservation>();
            string selectQuery = "SELECT Client_Id, Reservation_Id, Approved FROM Client_Reservation WHERE Client_Id = @ClientId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return list;
        }

        public async Task<List<ClientReservation>> GetByReservationId(int reservationId)
        {
            var list = new List<ClientReservation>();
            string selectQuery = "SELECT Client_Id, Reservation_Id, Approved FROM Client_Reservation WHERE Reservation_Id = @ReservationId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return list;
        }

        public async Task<ClientReservation?> Get(int clientId, int reservationId)
        {
            ClientReservation? clientReservation = null;
            string selectQuery = "SELECT Client_Id, Reservation_Id, Approved FROM Client_Reservation WHERE Client_Id = @ClientId AND Reservation_Id = @ReservationId";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
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

            CloseConnection();
            return clientReservation;
        }

        public async Task UpdateApproval(int clientId, int reservationId, bool approved)
        {
            string updateQuery = "UPDATE Client_Reservation SET Approved = @Approved WHERE Client_Id = @ClientId AND Reservation_Id = @ReservationId";
            OpenConnection();

            using (SqliteCommand command = new(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("@Approved", approved);
                command.Parameters.AddWithValue("@ClientId", clientId);
                command.Parameters.AddWithValue("@ReservationId", reservationId);
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }
    }
}

