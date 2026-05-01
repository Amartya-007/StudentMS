using Microsoft.Data.SqlClient;
using Serilog;
using System.Data;

namespace StudentMS.Common.CommonInfra
{
    public static class DBManager
    {
        private static string ConnectionString => ConnectionStrings.MainDB;

        /// <summary>
        /// Opens a SQL connection and returns a SqlCommand configured for a stored procedure.
        /// Caller is responsible for closing via CloseConnection().
        /// </summary>
        public static async Task<SqlCommand> GetStoredProcCommandAsync(string spName)
        {
            Log.Debug("DBManager: Opening connection for SP: {SP}", spName);
            var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            return new SqlCommand(spName, conn)
            {
                CommandType    = CommandType.StoredProcedure,
                CommandTimeout = 30
            };
        }

        /// <summary>
        /// Safely closes the connection associated with the given command.
        /// </summary>
        public static void CloseConnection(SqlCommand? cmd)
        {
            try
            {
                cmd?.Connection?.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DBManager.CloseConnection error");
            }
        }
    }
}
