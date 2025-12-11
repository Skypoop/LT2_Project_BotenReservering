using System;
using System.Collections.Generic;
using System.Data;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Seeders
{
    public class BoatSeeder : IDatabaseSeeder
    {
        public int Order => 1;

        public void Seed(IDbConnection connection)
        {
            if (!IsTableEmpty(connection)) return;

            List<Boat> boats = new List<Boat>
            {
                new Boat("Skiff van Kunststof", false, 1, 3, BoatType.S, 45, true, "Local Club"),
                new Boat("Dubbel Twee van Kunststof", false, 2, 3, BoatType.S, 46, true, "Local Club"),
                new Boat("Twee zonder van Kunststof", false, 2, 3, BoatType.B, 46, true, "Local Club"),
                new Boat("Twee met van Kunststof", true, 3, 2, BoatType.B, 46, true, "Local Club"),
                new Boat("Dubbel vier van Kunststof", false, 4, 3, BoatType.S, 50, true, "Local Club"),
                new Boat("Dubbel vier met van Kunststof", true, 5, 2, BoatType.B, 52, true, "Local Club"),
                new Boat("Vier zonder van Kunststof", false, 4, 3, BoatType.B, 50, true, "Local Club"),
                new Boat("Vier met van Kunststof", true, 5, 2, BoatType.B, 52, true, "Local Club"),
                new Boat("Acht van Kunststof", true, 9, 2, BoatType.B, 55, true, "Local Club")
            };

            foreach (Boat boat in boats)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Boat(Name, Steering_Wheel, Seats, Level, Type, Kg, Operational, Club) 
                                            VALUES(@Name, @SteeringWheel, @Seats, @Level, @Type, @Kg, @Operational, @Club)";
                    command.AddParameter("@Name", boat.Name);
                    command.AddParameter("@SteeringWheel", boat.SteeringWheel);
                    command.AddParameter("@Seats", boat.Seats);
                    command.AddParameter("@Level", boat.Level);
                    command.AddParameter("@Type", boat.Type.ToString());
                    command.AddParameter("@Kg", boat.Kg);
                    command.AddParameter("@Operational", boat.Operational);
                    command.AddParameter("@Club", (object?)boat.Club ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool IsTableEmpty(IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Boat";
                long count = Convert.ToInt64(command.ExecuteScalar());
                return count == 0;
            }
        }
    }
}