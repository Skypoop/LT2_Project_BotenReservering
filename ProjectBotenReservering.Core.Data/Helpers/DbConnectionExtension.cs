using System.Data;

namespace ProjectBotenReservering.Core.Data.Helpers
{
    public static class DbConnectionExtensions
    {
        public static bool IsTableEmpty(this IDbConnection connection, string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName) || tableName.Contains(";") || tableName.Contains("'"))
            {
                throw new ArgumentException("Invalid table name.", nameof(tableName));
            }

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                long count = Convert.ToInt64(command.ExecuteScalar());
                return count == 0;
            }
        }
    }
}