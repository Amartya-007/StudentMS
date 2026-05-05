using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Interfaces
{
    public interface IUserInfra
    {
        /// <summary>Returns the user record AND the raw PasswordHash for BCrypt verification.</summary>
        Task<(UserResponseModel? user, string? passwordHash)> GetUserWithHashByUsername(UserDomainModel request);

        /// <summary>Inserts a new user with a pre-hashed password. Returns new UserId.</summary>
        Task<int> CreateUser(UserDomainModel request);

        /// <summary>Increments FailedLoginCount by 1 for the given UserId.</summary>
        Task<bool> UpdateFailedLoginCount(UserDomainModel request);

        /// <summary>Sets LockoutUntil and FailedLoginCount = 5 for the given UserId.</summary>
        Task<bool> SetLockout(UserDomainModel request);

        /// <summary>Resets FailedLoginCount to 0 and LockoutUntil to NULL for the given UserId.</summary>
        Task<bool> ResetLockout(UserDomainModel request);

        /// <summary>Updates the PasswordHash column for the given UserId.</summary>
        Task<bool> UpdatePasswordHash(UserDomainModel request);

        /// <summary>Returns all users with all management columns.</summary>
        Task<List<UserResponseModel>> GetAllUsers(UserDomainModel request);

        /// <summary>Sets IsActive and Status for the given UserId.</summary>
        Task<bool> SetUserActiveStatus(UserDomainModel request);

        /// <summary>Returns true if the given username already exists in the Users table.</summary>
        Task<bool> CheckUsernameExists(UserDomainModel request);

        /// <summary>Returns a single user by UserId, or null if not found.</summary>
        Task<UserResponseModel?> GetUserById(UserDomainModel request);

        /// <summary>Sets Teachers.UserId for the given TeacherId.</summary>
        Task<bool> LinkUserToTeacher(UserDomainModel request);
    }
}
