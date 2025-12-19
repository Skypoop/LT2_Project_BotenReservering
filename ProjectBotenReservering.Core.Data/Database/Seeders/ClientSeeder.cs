using System.Data;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Seeders
{
    public class ClientSeeder : IDatabaseSeeder
    {
        public int Order => 2;

        public void Seed(IDbConnection connection)
        {
            if (!connection.IsTableEmpty("Client")) return;

            List<Client> clients = new List<Client>
            {
                new Client("Joe Doe", "joe.doe@example.com", 1, 2, "Remus Invictus", true,  PasswordHelper.HashPassword("hash1"),  0),
                new Client("Jane Smith", "jane.smith@example.com", 2, 1, "Remus Invictus", false, PasswordHelper.HashPassword("hash2"),  0),
                new Client("Bob Brown", "bob.brown@example.com", 3, 3, "Remus Invictus", false, PasswordHelper.HashPassword("hash3"),  0),
                new Client("Alice Green", "alice.green@example.com", 0, 1, "Remus Invictus", true,  PasswordHelper.HashPassword("hash4"),  0),
                new Client("Eve White", "eve.white@example.com", 1, 0, "Remus Invictus", false, PasswordHelper.HashPassword("hash5"),  0),

                new Client("Charlie Black", "charlie.black@example.com", 2, 2, "Remus Invictus", true,  PasswordHelper.HashPassword("hash6"),  0),
                new Client("Diana Blue", "diana.blue@example.com", 3, 1, "Remus Invictus", false, PasswordHelper.HashPassword("hash7"),  0),
                new Client("Frank Yellow", "frank.yellow@example.com", 1, 3, "Remus Invictus", true,  PasswordHelper.HashPassword("hash8"),  0),
                new Client("Grace Miller", "grace.miller@example.com", 0, 2, "Remus Invictus", false, PasswordHelper.HashPassword("hash9"),  0),
                new Client("Henry Wilson", "henry.wilson@example.com", 2, 0, "Remus Invictus", true,  PasswordHelper.HashPassword("hash10"), 0),

                new Client("Isabel Moore", "isabel.moore@example.com", 1, 1, "Remus Invictus", false, PasswordHelper.HashPassword("hash11"), 0),
                new Client("Jack Taylor", "jack.taylor@example.com", 3, 2, "Remus Invictus", true,  PasswordHelper.HashPassword("hash12"), 0),
                new Client("Karen Anderson", "karen.anderson@example.com", 0, 0, "Remus Invictus", false, PasswordHelper.HashPassword("hash13"), 0),
                new Client("Liam Thomas", "liam.thomas@example.com", 2, 3, "Remus Invictus", true,  PasswordHelper.HashPassword("hash14"), 0),
                new Client("Mia Jackson", "mia.jackson@example.com", 1, 2, "Remus Invictus", false, PasswordHelper.HashPassword("hash15"), 0),

                new Client("Noah Harris", "noah.harris@example.com", 3, 1, "Remus Invictus", true,  PasswordHelper.HashPassword("hash16"), 0),
                new Client("Olivia Martin", "olivia.martin@example.com", 0, 3, "Remus Invictus", false, PasswordHelper.HashPassword("hash17"), 0),
                new Client("Paul Thompson", "paul.thompson@example.com", 2, 2, "Remus Invictus", true,  PasswordHelper.HashPassword("hash18"), 0),
                new Client("Quinn Garcia", "quinn.garcia@example.com", 1, 0, "Remus Invictus", false, PasswordHelper.HashPassword("hash19"), 0),
                new Client("Rachel Martinez", "rachel.martinez@example.com", 3, 3, "Remus Invictus", true,  PasswordHelper.HashPassword("hash20"), 0),
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