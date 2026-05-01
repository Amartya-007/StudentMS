using StudentMS.Common.Models;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Business.Interfaces
{
    public interface IDepartmentBusiness
    {
        Task<AppResult<List<DepartmentResponseModel>>> GetDepartments(DepartmentRequestModel request);
        Task<AppResult<DepartmentResponseModel>>       GetDepartmentById(DepartmentRequestModel request);
        Task<AppResult<int>>                           InsertDepartment(DepartmentRequestModel request);
        Task<AppResult<bool>>                          UpdateDepartment(DepartmentRequestModel request);
        Task<AppResult<bool>>                          DeleteDepartment(DepartmentRequestModel request);
    }
}
