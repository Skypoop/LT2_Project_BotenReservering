
using System.Data.Common;


namespace Grocery.Core.Data.Helpers
{
    public static class DbConnectionExtensions
    {
        public static int ExecuteNonQuery(this DbConnection connection, string commandText, int timeout = 30)
        {
            var command = connection.CreateCommand();
            command.CommandTimeout = timeout;
            command.CommandText = commandText;
            return command.ExecuteNonQuery();
        }

        public static DbDataReader ExecuteReader(this DbConnection connection, string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteReader();
        }
    }
}
