using BCrypt.Net;
using Serilog;
using StudentMS.Business.Base;
using StudentMS.Business.Interfaces;
using StudentMS.Common.Logging;
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
        private readonly IActivityLogBusiness _logger;

        public UserBusiness(IUserInfra infra, IActivityLogBusiness logger)
        {
            _infra  = infra;
            _logger = logger;
        }

        // ── ValidateUser (AuthService + LockoutGuard) ─────────────────────────

        /// <summary>
        /// Authenticates a user. Enforces account status checks BEFORE BCrypt verification
        /// (Req 9.4), applies lockout logic (Req 4), and resets lockout on success (Req 4.4).
        /// </summary>
        public async Task<AppResult<UserResponseModel>> ValidateUser(UserRequestModel request)
        {
            var result = new AppResult<UserResponseModel>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    return Fail<UserResponseModel>("Username is required.");
                if (string.IsNullOrWhiteSpace(request.Password))
                    return Fail<UserResponseModel>("Password is required.");

                var domain = CreateRequest<UserDomainModel>(request);
                domain.Username = request.Username;

                // Fetch user + hash from DB
                var (user, storedHash) = await _infra.GetUserWithHashByUsername(domain);

                // Req 1.4 / 4.5: User not found — return generic message, no side effects
                if (user == null)
                {
                    Log.Warning("UserBusiness.ValidateUser: Username not found: {Username}", request.Username);
                    _logger.LogActivity(ActivityActions.LoginFailure, $"Failed login attempt for '{request.Username}'", overrideUsername: request.Username);
                    return Fail<UserResponseModel>("Invalid username or password.");
                }

                // Req 9.4: Evaluate account status BEFORE password check

                // Req 9.1 / 9.3: IsActive = 0 or Status = 'Inactive'
                if (!user.IsActive || user.Status == "Inactive")
                {
                    Log.Warning("UserBusiness.ValidateUser: Inactive account: {Username}", request.Username);
                    return Fail<UserResponseModel>("Your account is inactive. Please contact an administrator.");
                }

                // Req 9.2: Status = 'Suspended'
                if (user.Status == "Suspended")
                {
                    Log.Warning("UserBusiness.ValidateUser: Suspended account: {Username}", request.Username);
                    return Fail<UserResponseModel>("Your account has been suspended. Please contact an administrator.");
                }

                // Req 4.3: Account locked (LockoutUntil in the future)
                if (user.LockoutUntil.HasValue && user.LockoutUntil.Value > DateTime.UtcNow)
                {
                    Log.Warning("UserBusiness.ValidateUser: Locked account: {Username}", request.Username);
                    return Fail<UserResponseModel>("Account locked. Too many failed attempts. Try again after 15 minutes.");
                }

                // Req 8.5: NULL or empty hash — treat as failure
                if (string.IsNullOrEmpty(storedHash))
                {
                    Log.Warning("UserBusiness.ValidateUser: NULL/empty hash for {Username}", request.Username);
                    return Fail<UserResponseModel>("Invalid username or password.");
                }

                // Req 1.1 / 8.3: Verify plain-text password against BCrypt hash
                bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, storedHash);

                if (!passwordValid)
                {
                    Log.Warning("UserBusiness.ValidateUser: Failed login for {Username}", request.Username);

                    _logger.LogActivity(ActivityActions.LoginFailure, $"Failed login attempt for '{request.Username}'", overrideUsername: request.Username);

                    // Req 4.1: Increment FailedLoginCount
                    int newCount = user.FailedLoginCount + 1;
                    var lockDomain = CreateRequest<UserDomainModel>(request);
                    lockDomain.UserId = user.UserId;

                    if (newCount >= 5)
                    {
                        // Req 4.2: Lock the account for 15 minutes
                        lockDomain.LockoutUntil = DateTime.UtcNow.AddMinutes(15);
                        await _infra.SetLockout(lockDomain);
                        return Fail<UserResponseModel>("Account locked. Too many failed attempts. Try again after 15 minutes.");
                    }
                    else
                    {
                        await _infra.UpdateFailedLoginCount(lockDomain);
                        return Fail<UserResponseModel>("Invalid username or password.");
                    }
                }

                // Req 4.4: Successful login — reset lockout state
                var resetDomain = CreateRequest<UserDomainModel>(request);
                resetDomain.UserId = user.UserId;
                await _infra.ResetLockout(resetDomain);

                Log.Information("UserBusiness.ValidateUser: Login succeeded for {Username}", request.Username);
                result.Status       = true;
                result.ResponseData = user;
                result.ErrorCode    = ErrorCodes.Success;
                result.Message      = "Login successful.";

                _logger.LogActivity(ActivityActions.LoginSuccess, $"User '{request.Username}' logged in successfully");
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

        // ── CreateUser ────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a new user with a BCrypt-hashed password.
        /// Used internally; for teacher account creation use AccountManagerBusiness.
        /// </summary>
        public async Task<AppResult<int>> CreateUser(UserRequestModel request)
        {
            var result = new AppResult<int>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    return Fail<int>("Username is required.");
                if (string.IsNullOrWhiteSpace(request.Password))
                    return Fail<int>("Password is required.");
                if (request.Password.Length < 8)
                    return Fail<int>("Password must be at least 8 characters.");

                // Req 8.2: Hash with BCrypt work factor 12
                string hash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

                var domain = CreateRequest<UserDomainModel>(request);
                domain.Username         = request.Username;
                domain.PasswordHash     = hash;
                domain.Role             = request.Role ?? "Staff";
                domain.IsActive         = true;
                domain.Status           = "Active";
                domain.FailedLoginCount = 0;

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

        // ── ChangePassword (PasswordChanger) ──────────────────────────────────

        /// <summary>
        /// Allows a teacher to change their own password.
        /// Validates current password, length, confirmation, and same-as-current rules.
        /// </summary>
        public async Task<AppResult<bool>> ChangePassword(ChangePasswordRequestModel request)
        {
            var result = new AppResult<bool>();
            try
            {
                // Req 5.3: New password length
                if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
                    return Fail<bool>("New password must be at least 8 characters.");

                // Req 5.4: Confirmation must match
                if (request.NewPassword != request.ConfirmPassword)
                    return Fail<bool>("New password and confirmation do not match.");

                // Fetch current hash
                var domain = CreateRequest<UserDomainModel>(request);
                domain.TargetUserId = request.TargetUserId;
                domain.UserId       = request.TargetUserId;

                var user = await _infra.GetUserById(domain);
                if (user == null)
                    return Fail<bool>("User not found.");

                // Need the hash — fetch via username lookup using GetUserById result
                // Re-fetch with username to get hash
                var hashDomain = CreateRequest<UserDomainModel>(request);
                hashDomain.Username = user.Username;
                var (_, storedHash) = await _infra.GetUserWithHashByUsername(hashDomain);

                if (string.IsNullOrEmpty(storedHash))
                    return Fail<bool>("Current password is incorrect.");

                // Req 5.1 / 5.2: Verify current password
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, storedHash))
                    return Fail<bool>("Current password is incorrect.");

                // Req 5.5: New must differ from current
                if (BCrypt.Net.BCrypt.Verify(request.NewPassword, storedHash))
                    return Fail<bool>("New password must differ from the current password.");

                // Req 5.6 / 8.2: Hash new password with work factor 12
                string newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);

                var updateDomain = CreateRequest<UserDomainModel>(request);
                updateDomain.UserId       = request.TargetUserId;
                updateDomain.PasswordHash = newHash;
                await _infra.UpdatePasswordHash(updateDomain);

                Log.Information("UserBusiness.ChangePassword: Password changed for UserId={UserId}", request.TargetUserId);
                result.Status       = true;
                result.ResponseData = true;
                result.ErrorCode    = ErrorCodes.Success;
                result.Message      = "Password changed successfully.";

                _logger.LogActivity(ActivityActions.PasswordChange, $"User changed their password", "User", request.TargetUserId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserBusiness.ChangePassword: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while changing password.";
            }
            return result;
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private static AppResult<T> Fail<T>(string message)
            => new() { Status = false, ErrorCode = ErrorCodes.ValidationFailed, Message = message };
    }
}
