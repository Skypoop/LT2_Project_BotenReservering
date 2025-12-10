using Microsoft.Data.Sqlite;
using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientRepository : DatabaseConnection, IClientRepository
    {
        public ClientRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Client (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [Full_Name] VARCHAR NOT NULL,
                            [Email] VARCHAR NOT NULL UNIQUE,
                            [Scull_level] INT,
                            [Sweep_level] INT,
                            [Club] VARCHAR,
                            [Approved] BOOLEAN NOT NULL DEFAULT 0,
                            [Password_Hash] VARCHAR NOT NULL)");

            List<Client> clients = GetAll();
            bool anyClientExists = clients.Count > 0;
            if (!anyClientExists)
            {
                // demo clients to populate the 
                Add(new Client("Joe Doe", "joe.doe@example.com", 1, 2, "River Club", true, PasswordHelper.HashPassword("hash1"), 0));
                Add(new Client("Jane Smith", "jane.smith@example.com", 2, 1, "Lakeside Club", false, PasswordHelper.HashPassword("hash2"), 0));
                Add(new Client("Bob Brown", "bob.brown@example.com", 3, 3, null, false, PasswordHelper.HashPassword("hash3"), 0));
                Add(new Client("Alice Green", "alice.green@example.com", 0, 1, "Harbor Club", true, PasswordHelper.HashPassword("hash4"), 0));
                Add(new Client("Eve White", "eve.white@example.com", 1, 0, "Coast Club", false, PasswordHelper.HashPassword("hash5"), 0));

            }
        }

        public Client Add(Client item)
        {
            string insertQuery = @"INSERT INTO Client(Full_Name, Email, Scull_level, Sweep_level, Club, Approved, Password_Hash) 
                                   VALUES(@FullName, @Email, @ScullLevel, @SweepLevel, @Club, @Approved, @PasswordHash);
                                   SELECT last_insert_rowid();";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@FullName", item.FullName);
                command.Parameters.AddWithValue("@Email", item.Email);
                command.Parameters.AddWithValue("@ScullLevel", item.ScullLevel);
                command.Parameters.AddWithValue("@SweepLevel", item.SweepLevel);
                command.Parameters.AddWithValue("@Club", item.Club ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Approved", item.Approved);
                command.Parameters.AddWithValue("@PasswordHash", item.PasswordHash);

                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public Client? Get(string email)
        {
            Client? client = null;
            string selectQuery = "SELECT Id, Full_Name, Email, Scull_level, Sweep_level, Club, Approved, Password_Hash FROM Client WHERE Email = @Email";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        client = MapReaderToClient(reader);
                    }
                }
            }

            CloseConnection();
            return client;
        }

        public Client? Get(int id)
        {
            Client? client = null;
            string selectQuery = "SELECT Id, Full_Name, Email, Scull_level, Sweep_level, Club, Approved, Password_Hash FROM Client WHERE Id = @Id";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        client = MapReaderToClient(reader);
                    }
                }
            }

            CloseConnection();
            return client;
        }

        public List<Client> GetAll()
        {
            List<Client> clientList = new List<Client>();
            string selectQuery = "SELECT * FROM Client";
            OpenConnection();

            using (SqliteCommand command = new(selectQuery, Connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientList.Add(MapReaderToClient(reader));
                    }
                }
            }

            CloseConnection();
            return clientList;
        }

        private Client MapReaderToClient(SqliteDataReader reader)
        {
            return new Client(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetInt32(3),
                reader.GetInt32(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.GetBoolean(6),
                reader.GetString(7),
                reader.GetInt32(0)
            );
        }
    }
}
