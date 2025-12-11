using System.Data;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Data.Database.Fixtures
{
    public class ReservationFixture : IDatabaseFixture
    {
        public int Order => 3;

        public void Seed(IDbConnection connection)
        {
            if (!connection.IsTableEmpty("Reservation")) return;

            DateTime now = DateTime.Now;
            List<Reservation> reservations = new List<Reservation>
            {
                new Reservation(now, now.AddHours(2), now.AddHours(4), 1, 1, true),
                new Reservation(now, now.AddDays(1).AddHours(10), now.AddDays(1).AddHours(11), 1, 1, true),
                new Reservation(now, now.AddDays(3).AddHours(14), now.AddDays(3).AddHours(16), 3, 1, true),
                new Reservation(now, now.AddDays(5).AddHours(9), now.AddDays(5).AddHours(10).AddMinutes(30), 1, 1, true),
                new Reservation(now, now.AddDays(12).AddHours(16), now.AddDays(12).AddHours(18), 2, 1, true),
                new Reservation(now, now.AddDays(18).AddHours(8), now.AddDays(18).AddHours(10), 3, 1, true),
                new Reservation(now, now.AddDays(21).AddHours(11), now.AddDays(21).AddHours(12), 1, 1, true),
                new Reservation(now, now.AddDays(24).AddHours(13), now.AddDays(24).AddHours(15), 2, 1, true),
                new Reservation(now, now.AddDays(28).AddHours(15), now.AddDays(28).AddHours(16).AddMinutes(30), 1, 1, true),
                new Reservation(now, now.AddDays(30).AddHours(10), now.AddDays(30).AddHours(12), 1, 1, true),
                new Reservation(now, now.AddHours(1), now.AddHours(4), 1, 2, true),
                new Reservation(now, now.AddDays(1).AddHours(9), now.AddDays(1).AddHours(11), 2, 2, true),
                new Reservation(now, now.AddDays(3).AddHours(15), now.AddDays(3).AddHours(17), 1, 2, true),
                new Reservation(now, now.AddDays(5).AddHours(8), now.AddDays(5).AddHours(10).AddMinutes(10), 1, 2, true),
                new Reservation(now, now.AddDays(12).AddHours(2), now.AddDays(12).AddHours(1), 3, 2, true),
                new Reservation(now, now.AddDays(18).AddHours(3), now.AddDays(18).AddHours(5), 2, 2, true),
                new Reservation(now, now.AddDays(21).AddHours(7), now.AddDays(21).AddHours(12), 1, 2, true),
                new Reservation(now, now.AddDays(24).AddHours(13), now.AddDays(24).AddHours(15), 2, 2, true),
                new Reservation(now, now.AddDays(28).AddHours(15), now.AddDays(28).AddHours(16).AddMinutes(30), 1, 2, true),
                new Reservation(now, now.AddDays(30).AddHours(10), now.AddDays(30).AddHours(12), 2, 2, true)
            };

            foreach (Reservation res in reservations)
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Reservation(Created_At, Start_Time, End_Time, Client_Id, Boat_Id, Approved, Active) 
                                            VALUES(@CreatedAt, @StartTime, @EndTime, @ClientId, @BoatId, @Approved, @Active)";
                    command.AddParameter("@CreatedAt", res.CreatedAt);
                    command.AddParameter("@StartTime", res.StartTime);
                    command.AddParameter("@EndTime", res.EndTime);
                    command.AddParameter("@ClientId", res.ClientId);
                    command.AddParameter("@BoatId", res.BoatId);
                    command.AddParameter("@Approved", res.Approved);
                    command.AddParameter("@Active", res.Active);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}