using Microsoft.Data.SqlClient;
using Serilog;
using StudentMS.Common.CommonInfra;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Services
{
    public class UserInfra : IUserInfra
    {
        /// <summary>
        /// Fetches the user record AND the raw PasswordHash column so the business layer
        /// can run BCrypt.Verify without exposing the hash in the public response model.
        /// </summary>
        public async Task<(UserResponseModel? user, string? passwordHash)> GetUserWithHashByUsername(UserDomainModel request)
        {
            Log.Debug("UserInfra.GetUserWithHashByUsername: Username={Username}", request.Username);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.GetUserByUsername);

                // Only pass @Username — do NOT use AddSQLParameters here because
                // UserDomainModel also has @PasswordHash and @Role which USP_GetUserByUsername
                // does not accept, causing a SqlException.
                dbCommand.Parameters.AddWithValue("@Username", request.Username ?? (object)DBNull.Value);

                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);

                if (await reader.ReadAsync())
                {
                    var user = new UserResponseModel
                    {
                        UserId   = DbHelper.GetInt32(reader, ord, "UserId"),
                        Username = DbHelper.GetString(reader, ord, "Username"),
                        Role     = DbHelper.GetString(reader, ord, "Role"),
                        IsActive = DbHelper.GetBool(reader, ord, "IsActive")
                    };

                    // Read hash separately — never exposed in UserResponseModel
                    string? hash = ord.ContainsKey("PasswordHash")
                        ? DbHelper.CheckDbNullString(reader.GetValue(ord["PasswordHash"]))
                        : null;

                    return (user, hash);
                }

                return (null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.GetUserWithHashByUsername: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        /// <summary>
        /// Inserts a new user. The PasswordHash must already be BCrypt-hashed by the business layer.
        /// Returns the new UserId.
        /// </summary>
        public async Task<int> CreateUser(UserDomainModel request)
        {
            Log.Debug("UserInfra.CreateUser: Username={Username}", request.Username);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.CreateUser);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return DbHelper.CheckDbNullInt(await dbCommand.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.CreateUser: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }
    }
}
