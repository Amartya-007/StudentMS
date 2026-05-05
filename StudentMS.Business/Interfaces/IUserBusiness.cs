using StudentMS.Common.Models;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Business.Interfaces
{
    public interface IUserBusiness
    {
        Task<AppResult<UserResponseModel>> ValidateUser(UserRequestModel request);
        Task<AppResult<int>>              CreateUser(UserRequestModel request);
        Task<AppResult<bool>>             ChangePassword(ChangePasswordRequestModel request);
    }
}
