using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Interfaces
{
    public interface IDepartmentInfra
    {
        Task<List<DepartmentResponseModel>> GetDepartments(DepartmentDomainModel request);
        Task<DepartmentResponseModel?>      GetDepartmentById(DepartmentDomainModel request);
        Task<int>                           InsertDepartment(DepartmentDomainModel request);
        Task<bool>                          UpdateDepartment(DepartmentDomainModel request);
        Task<bool>                          DeleteDepartment(DepartmentDomainModel request);
    }
}
