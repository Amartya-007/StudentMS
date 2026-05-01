using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Interfaces
{
    public interface IStudentInfra
    {
        Task<List<StudentResponseModel>> GetStudents(StudentDomainModel request);
        Task<StudentResponseModel?>      GetStudentById(StudentDomainModel request);
        Task<int>                        InsertStudent(StudentDomainModel request);
        Task<bool>                       UpdateStudent(StudentDomainModel request);
        Task<bool>                       DeleteStudent(StudentDomainModel request);
    }
}
