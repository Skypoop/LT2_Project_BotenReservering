using System;
using System.Data;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Interfaces.Database;

namespace ProjectBotenReservering.Core.Data.Database.Seeders
{
    public class WindConstraintSeeder : IDatabaseSeeder
    {
        public int Order => 1;

        public void Seed(IDbConnection connection)
        {
            if (!IsTableEmpty(connection)) return;

            for (int i = 1; i < 12; i++)
            {
                int minLevel = Math.Clamp((int)MathF.Ceiling((i + 2) / 2f), 2, 4);
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO WindConstraint(Windforce, Min_Scull_level, Min_Sweep_level) 
                                            VALUES(@Windforce, @MinScullLevel, @MinSweepLevel)";
                    command.AddParameter("@Windforce", i);
                    command.AddParameter("@MinScullLevel", minLevel);
                    command.AddParameter("@MinSweepLevel", minLevel);
                    command.ExecuteNonQuery();
                }
            }
        }

        private bool IsTableEmpty(IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM WindConstraint";
                long count = Convert.ToInt64(command.ExecuteScalar());
                return count == 0;
            }
        }
    }
}