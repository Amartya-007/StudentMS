using StudentMS.Common.Models;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Business.Interfaces
{
    public interface ITeacherBusiness
    {
        Task<AppResult<List<TeacherResponseModel>>> GetTeachers(TeacherRequestModel request);
        Task<AppResult<TeacherResponseModel>>       GetTeacherById(TeacherRequestModel request);
        Task<AppResult<int>>                        InsertTeacher(TeacherRequestModel request);
        Task<AppResult<bool>>                       UpdateTeacher(TeacherRequestModel request);
        Task<AppResult<bool>>                       DeleteTeacher(TeacherRequestModel request);
    }
}
