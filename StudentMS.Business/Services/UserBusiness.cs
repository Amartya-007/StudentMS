using BCrypt.Net;
using Serilog;
using StudentMS.Business.Base;
using StudentMS.Business.Interfaces;
using StudentMS.Common.Models;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Models.DomainModels;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Business.Services
{
    public class UserBusiness : BusinessServiceBase, IUserBusiness
    {
        private readonly IUserInfra _infra;

        public UserBusiness(IUserInfra infra) => _infra = infra;

        /// <summary>
        /// Validates username/password. Fetches the stored BCrypt hash from DB
        /// and verifies the plain-text password against it.
        /// </summary>
        public async Task<AppResult<UserResponseModel>> ValidateUser(UserRequestModel request)
        {
            var result = new AppResult<UserResponseModel>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    return Fail("Username is required.");
                if (string.IsNullOrWhiteSpace(request.Password))
                    return Fail("Password is required.");

                var domain = CreateRequest<UserDomainModel>(request);
                domain.Username = request.Username;

                // Fetch user + hash from DB
                var (user, storedHash) = await _infra.GetUserWithHashByUsername(domain);

                if (user == null || !user.IsActive || string.IsNullOrEmpty(storedHash))
                {
                    result.Status    = false;
                    result.ErrorCode = ErrorCodes.Unauthorized;
                    result.Message   = "Invalid username or password.";
                    return result;
                }

                // Verify plain-text password against BCrypt hash
                bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, storedHash);

                if (!passwordValid)
                {
                    Log.Warning("UserBusiness.ValidateUser: Failed login attempt for {Username}", request.Username);
                    result.Status    = false;
                    result.ErrorCode = ErrorCodes.Unauthorized;
                    result.Message   = "Invalid username or password.";
                    return result;
                }

                Log.Information("UserBusiness.ValidateUser: Login succeeded for {Username}", request.Username);
                result.Status       = true;
                result.ResponseData = user;
                result.ErrorCode    = ErrorCodes.Success;
                result.Message      = "Login successful.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserBusiness.ValidateUser: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred during login.";
            }
            return result;
        }

        /// <summary>
        /// Creates a new user with a BCrypt-hashed password.
        /// </summary>
        public async Task<AppResult<int>> CreateUser(UserRequestModel request)
        {
            var result = new AppResult<int>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    return FailInt("Username is required.");
                if (string.IsNullOrWhiteSpace(request.Password))
                    return FailInt("Password is required.");
                if (request.Password.Length < 6)
                    return FailInt("Password must be at least 6 characters.");

                // Hash the password with BCrypt (work factor 12)
                string hash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

                var domain = CreateRequest<UserDomainModel>(request);
                domain.Username     = request.Username;
                domain.PasswordHash = hash;
                domain.Role         = request.Role ?? "Staff";

                int newId = await _infra.CreateUser(domain);
                result.Status       = newId > 0;
                result.ResponseData = newId;
                result.ErrorCode    = newId > 0 ? ErrorCodes.Success : ErrorCodes.DatabaseError;
                result.Message      = newId > 0 ? "User created successfully." : "Failed to create user.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserBusiness.CreateUser: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while creating user.";
            }
            return result;
        }

        private static AppResult<UserResponseModel> Fail(string message)
            => new() { Status = false, ErrorCode = ErrorCodes.ValidationFailed, Message = message };

        private static AppResult<int> FailInt(string message)
            => new() { Status = false, ErrorCode = ErrorCodes.ValidationFailed, Message = message };
    }
}
