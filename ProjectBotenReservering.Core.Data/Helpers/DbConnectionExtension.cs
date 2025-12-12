using System.Data;

namespace ProjectBotenReservering.Core.Data.Helpers
{
    public static class DbConnectionExtensions
    {
        private static readonly HashSet<string> _allowedTables =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Boat",
                "Role",
                "ManagementTask",
                "WindConstraint",
                "Client",
                "Reservation",
                "DamageReport",
                "DamageReportPhotos",
                "Match",
                "Client_ManagementTask",
                "Client_Reservation",
                "Client_Role",
                "Role_ManagementTask",
                "Reservation_Match"
            };

        public static bool IsTableEmpty(this IDbConnection connection, string tableName)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (!_allowedTables.Contains(tableName))
                throw new ArgumentException("Invalid or unknown table name.", nameof(tableName));

            bool shouldClose = false;

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                shouldClose = true;
            }

            try
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT COUNT(*) FROM [{tableName}];";
                    long count = Convert.ToInt64(command.ExecuteScalar());
                    return count == 0;
                }
            }
            finally
            {
                if (shouldClose)
                    connection.Close();
            }
        }
    }
}