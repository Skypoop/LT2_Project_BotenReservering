
using System.Data.Common;


namespace ProjectBotenReservering.Core.Data.Helpers
{
    public static class DbConnectionExtensions
    {
        public static int ExecuteNonQuery(this DbConnection connection, string commandText, int timeout = 30)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = timeout;
            command.CommandText = commandText;
            return command.ExecuteNonQuery();
        }

        public static DbDataReader ExecuteReader(this DbConnection connection, string commandText)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteReader();
        }
    }
}
