using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Interfaces
{
    public interface ITeacherInfra
    {
        Task<List<TeacherResponseModel>> GetTeachers(TeacherDomainModel request);
        Task<TeacherResponseModel?>      GetTeacherById(TeacherDomainModel request);
        Task<int>                        InsertTeacher(TeacherDomainModel request);
        Task<bool>                       UpdateTeacher(TeacherDomainModel request);
        Task<bool>                       DeleteTeacher(TeacherDomainModel request);
    }
}
