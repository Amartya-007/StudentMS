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
        // ── Helper: build ordinal dictionary from reader ─────────────────────

        private static Dictionary<string, int> GetOrdinals(SqlDataReader reader)
            => Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);

        // ── Helper: map a reader row to UserResponseModel ────────────────────

        private static UserResponseModel MapUser(SqlDataReader reader, Dictionary<string, int> ord)
            => new()
            {
                UserId           = DbHelper.GetInt32(reader, ord, "UserId"),
                Username         = DbHelper.GetString(reader, ord, "Username"),
                Role             = DbHelper.GetString(reader, ord, "Role"),
                IsActive         = DbHelper.GetBool(reader, ord, "IsActive"),
                Status           = DbHelper.GetString(reader, ord, "Status"),
                FailedLoginCount = DbHelper.GetInt32(reader, ord, "FailedLoginCount"),
                LockoutUntil     = DbHelper.GetDate(reader, ord, "LockoutUntil"),
                TeacherId        = ord.ContainsKey("TeacherId")
                                       ? (int?)DbHelper.GetInt32(reader, ord, "TeacherId")
                                       : null,
                CreatedOn        = DbHelper.GetDate(reader, ord, "CreatedOn") ?? DateTime.MinValue
            };

        // ── GetUserWithHashByUsername ─────────────────────────────────────────

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
                dbCommand.Parameters.AddWithValue("@Username", request.Username ?? (object)DBNull.Value);

                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = GetOrdinals(reader);

                if (await reader.ReadAsync())
                {
                    var user = MapUser(reader, ord);

                    string? hash = ord.ContainsKey("PasswordHash")
                        ? DbHelper.CheckDbNullString(reader.GetValue(ord["PasswordHash"]))
                        : null;

                    return (user, string.IsNullOrEmpty(hash) ? null : hash);
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

        // ── CreateUser ────────────────────────────────────────────────────────

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
                dbCommand.Parameters.AddWithValue("@Username",         request.Username     ?? (object)DBNull.Value);
                dbCommand.Parameters.AddWithValue("@PasswordHash",     request.PasswordHash ?? (object)DBNull.Value);
                dbCommand.Parameters.AddWithValue("@Role",             request.Role         ?? (object)DBNull.Value);
                dbCommand.Parameters.AddWithValue("@IsActive",         request.IsActive);
                dbCommand.Parameters.AddWithValue("@Status",           request.Status       ?? (object)DBNull.Value);
                dbCommand.Parameters.AddWithValue("@FailedLoginCount", request.FailedLoginCount);
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

        // ── UpdateFailedLoginCount ────────────────────────────────────────────

        public async Task<bool> UpdateFailedLoginCount(UserDomainModel request)
        {
            Log.Debug("UserInfra.UpdateFailedLoginCount: UserId={UserId}", request.UserId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.UpdateFailedLogin);
                dbCommand.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);
                await dbCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.UpdateFailedLoginCount: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── SetLockout ────────────────────────────────────────────────────────

        public async Task<bool> SetLockout(UserDomainModel request)
        {
            Log.Debug("UserInfra.SetLockout: UserId={UserId}, LockoutUntil={LockoutUntil}", request.UserId, request.LockoutUntil);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.SetLockout);
                dbCommand.Parameters.AddWithValue("@UserId",       request.UserId       ?? (object)DBNull.Value);
                dbCommand.Parameters.AddWithValue("@LockoutUntil", request.LockoutUntil ?? (object)DBNull.Value);
                await dbCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.SetLockout: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── ResetLockout ──────────────────────────────────────────────────────

        public async Task<bool> ResetLockout(UserDomainModel request)
        {
            Log.Debug("UserInfra.ResetLockout: UserId={UserId}", request.UserId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.ResetLockout);
                dbCommand.Parameters.AddWithValue("@UserId", request.UserId ?? (object)DBNull.Value);
                await dbCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.ResetLockout: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── UpdatePasswordHash ────────────────────────────────────────────────

        public async Task<bool> UpdatePasswordHash(UserDomainModel request)
        {
            Log.Debug("UserInfra.UpdatePasswordHash: UserId={UserId}", request.UserId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.UpdatePasswordHash);
                dbCommand.Parameters.AddWithValue("@UserId",       request.UserId       ?? (object)DBNull.Value);
                dbCommand.Parameters.AddWithValue("@PasswordHash", request.PasswordHash ?? (object)DBNull.Value);
                await dbCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.UpdatePasswordHash: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── GetAllUsers ───────────────────────────────────────────────────────

        public async Task<List<UserResponseModel>> GetAllUsers(UserDomainModel request)
        {
            Log.Debug("UserInfra.GetAllUsers");
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.GetAllUsers);
                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = GetOrdinals(reader);
                var list = new List<UserResponseModel>();
                while (await reader.ReadAsync())
                    list.Add(MapUser(reader, ord));
                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.GetAllUsers: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── SetUserActiveStatus ───────────────────────────────────────────────

        public async Task<bool> SetUserActiveStatus(UserDomainModel request)
        {
            Log.Debug("UserInfra.SetUserActiveStatus: UserId={UserId}, IsActive={IsActive}", request.TargetUserId, request.IsActive);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.SetUserActiveStatus);
                dbCommand.Parameters.AddWithValue("@UserId",   request.TargetUserId ?? (object)DBNull.Value);
                dbCommand.Parameters.AddWithValue("@IsActive", request.IsActive);
                dbCommand.Parameters.AddWithValue("@Status",   request.Status ?? (object)DBNull.Value);
                await dbCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.SetUserActiveStatus: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── CheckUsernameExists ───────────────────────────────────────────────

        public async Task<bool> CheckUsernameExists(UserDomainModel request)
        {
            Log.Debug("UserInfra.CheckUsernameExists: Username={Username}", request.Username);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.CheckUsernameExists);
                dbCommand.Parameters.AddWithValue("@Username", request.Username ?? (object)DBNull.Value);
                var result = await dbCommand.ExecuteScalarAsync();
                return DbHelper.CheckDbNullBool(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.CheckUsernameExists: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── GetUserById ───────────────────────────────────────────────────────

        public async Task<UserResponseModel?> GetUserById(UserDomainModel request)
        {
            Log.Debug("UserInfra.GetUserById: UserId={UserId}", request.TargetUserId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.GetUserById);
                dbCommand.Parameters.AddWithValue("@UserId", request.TargetUserId ?? (object)DBNull.Value);
                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = GetOrdinals(reader);
                if (await reader.ReadAsync())
                    return MapUser(reader, ord);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.GetUserById: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── LinkUserToTeacher ─────────────────────────────────────────────────

        public async Task<bool> LinkUserToTeacher(UserDomainModel request)
        {
            Log.Debug("UserInfra.LinkUserToTeacher: TeacherId={TeacherId}, UserId={UserId}", request.TeacherId, request.UserId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.UserSP.LinkUserToTeacher);
                dbCommand.Parameters.AddWithValue("@TeacherId", request.TeacherId  ?? (object)DBNull.Value);
                dbCommand.Parameters.AddWithValue("@UserId",    request.UserId     ?? (object)DBNull.Value);
                await dbCommand.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserInfra.LinkUserToTeacher: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }
    }
}
