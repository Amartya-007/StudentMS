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
    }
}
