using System;
using System.Data;

namespace ProjectBotenReservering.Core.Data.Helpers
{
    public static class DbCommandExtensions
    {
        public static void AddParameter(this IDbCommand command, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}