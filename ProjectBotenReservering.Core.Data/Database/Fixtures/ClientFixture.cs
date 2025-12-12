using System.Data;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Fixtures
{
    public class ClientFixture : IDatabaseFixture
    {
        public int Order => 2;

        public void Seed(IDbConnection connection)
        {
            if (!connection.IsTableEmpty("Client")) return;

            List<Client> clients = new List<Client>
            {
                new Client("Joe Doe", "joe.doe@example.com", 1, 2, "Remus Invictus", true, PasswordHelper.HashPassword("hash1"), 0),
                new Client("Jane Smith", "jane.smith@example.com", 2, 1, "Remus Invictus", false, PasswordHelper.HashPassword("hash2"), 0),
                new Client("Bob Brown", "bob.brown@example.com", 3, 3, "Remus Invictus", false, PasswordHelper.HashPassword("hash3"), 0),
                new Client("Alice Green", "alice.green@example.com", 0, 1, "Remus Invictus", true, PasswordHelper.HashPassword("hash4"), 0),
                new Client("Eve White", "eve.white@example.com", 1, 0, "Remus Invictus", false, PasswordHelper.HashPassword("hash5"), 0)
            };

            foreach (Client client in clients)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Client(Full_Name, Email, Scull_level, Sweep_level, Club, Approved, Password_Hash) 
                                            VALUES(@FullName, @Email, @ScullLevel, @SweepLevel, @Club, @Approved, @PasswordHash)";
                    command.AddParameter("@FullName", client.FullName);
                    command.AddParameter("@Email", client.Email);
                    command.AddParameter("@ScullLevel", client.ScullLevel);
                    command.AddParameter("@SweepLevel", client.SweepLevel);
                    command.AddParameter("@Club", (object?)client.Club ?? DBNull.Value);
                    command.AddParameter("@Approved", client.Approved);
                    command.AddParameter("@PasswordHash", client.PasswordHash);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}