using StudentMS.Common.Models;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Business.Interfaces
{
    public interface IStudentBusiness
    {
        Task<AppResult<List<StudentResponseModel>>> GetStudents(StudentRequestModel request);
        Task<AppResult<StudentResponseModel>>       GetStudentById(StudentRequestModel request);
        Task<AppResult<int>>                        InsertStudent(StudentRequestModel request);
        Task<AppResult<bool>>                       UpdateStudent(StudentRequestModel request);
        Task<AppResult<bool>>                       DeleteStudent(StudentRequestModel request);
    }
}
