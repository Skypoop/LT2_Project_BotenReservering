using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Data;
using Microsoft.Data.Sqlite;

namespace ProjectBotenReservering.Core.Data.Repositories
{
    public class ClientRepository : DatabaseConnection, IClientRepository
    {
        private readonly List<Client> clientList;

        public ClientRepository()
        {
            //ISO 8601 format: date.ToString("o", CultureInfo.InvariantCulture)
            CreateTable(@"CREATE TABLE IF NOT EXISTS Clients (
                            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            [FullName] NVARCHAR(80) NOT NULL,
                            [EmailAddress] NVARCHAR(100) NOT NULL UNIQUE,
                            [PasswordHash] NVARCHAR(256) NOT NULL)");
            List<string> insertQueries = [@"INSERT OR IGNORE INTO Clients(FullName, EmailAddress, PasswordHash) VALUES('Henk jansen', 'henk.jansen@gmail.com', 'None')",
                @"INSERT OR IGNORE INTO Clients(FullName, EmailAddress, PasswordHash) VALUES('Joe mama', 'Joe.Mama@gmail.com', 'None')",
                @"INSERT OR IGNORE INTO Clients(FullName, EmailAddress, PasswordHash) VALUES('Booi hoi', 'Booi.hoi@gmail.com', 'None')"];
            InsertMultipleWithTransaction(insertQueries);
  
            GetAll();
        }
        
        public Client Add(Client item)
        {
            int recordsAffected;
            string insertQuery = $"INSERT INTO Clients(FullName, EmailAddress, PasswordHash) VALUES(@FullName, @EmailAddress, @PasswordHash) Returning RowId;";
            OpenConnection();
            using (SqliteCommand command = new(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("FullName", item.FullName);
                command.Parameters.AddWithValue("EmailAddress", item.EmailAddress);
                command.Parameters.AddWithValue("PasswordHash", item.PasswordHash);

                //recordsAffected = command.ExecuteNonQuery();
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }
        
        public Client? Get(string email)
        {
            throw new NotImplementedException();
        }

        public Client? Get(int id)
        {
            throw new NotImplementedException();
        }

        public List<Client> GetAll()
        {
            clientList.Clear();
            string selectQuery = "SELECT Id, Name, date(Date), Color, ClientId FROM GroceryList";
            OpenConnection();
            
            using (SqliteCommand command = new(selectQuery, Connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string fullName = reader.GetString(1);
                    string emailAddress = reader.GetString(2);
                    string passwordHash = reader.GetString(3);
                    clientList.Add(new(id, fullName, emailAddress, passwordHash));
                }
            }
            
            CloseConnection();
            return clientList;
        }
    }
}