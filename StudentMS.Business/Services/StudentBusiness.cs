using Serilog;
using StudentMS.Business.Base;
using StudentMS.Business.Interfaces;
using StudentMS.Common.Models;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Models.DomainModels;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Business.Services
{
    public class StudentBusiness : BusinessServiceBase, IStudentBusiness
    {
        private readonly IStudentInfra _infra;

        public StudentBusiness(IStudentInfra infra) => _infra = infra;

        public async Task<AppResult<List<StudentResponseModel>>> GetStudents(StudentRequestModel request)
        {
            var result = new AppResult<List<StudentResponseModel>>();
            try
            {
                var domain = CreateRequest<StudentDomainModel>(request);
                domain.DepartmentId = request.DepartmentId;

                result.ResponseData = await _infra.GetStudents(domain);
                result.Status       = true;
                result.ErrorCode    = ErrorCodes.Success;
                result.Message      = "Students retrieved successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentBusiness.GetStudents: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while retrieving students.";
            }
            return result;
        }

        public async Task<AppResult<StudentResponseModel>> GetStudentById(StudentRequestModel request)
        {
            var result = new AppResult<StudentResponseModel>();
            try
            {
                var domain = CreateRequest<StudentDomainModel>(request);
                domain.StudentId = request.StudentId;

                var data = await _infra.GetStudentById(domain);
                result.Status       = data != null;
                result.ResponseData = data;
                result.ErrorCode    = data != null ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = data != null ? "Student found." : "Student not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentBusiness.GetStudentById: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred.";
            }
            return result;
        }

        public async Task<AppResult<int>> InsertStudent(StudentRequestModel request)
        {
            var result = new AppResult<int>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Fail<int>(ErrorCodes.ValidationFailed, "Student name is required.");
                if (request.DepartmentId <= 0)
                    return Fail<int>(ErrorCodes.ValidationFailed, "Please select a department.");

                var domain = CreateRequest<StudentDomainModel>(request);
                domain.Name         = request.Name;
                domain.DOB          = request.DOB;
                domain.Gender       = request.Gender;
                domain.Phone        = request.Phone;
                domain.Address      = request.Address;
                domain.DepartmentId = request.DepartmentId;

                int newId = await _infra.InsertStudent(domain);
                result.Status       = newId > 0;
                result.ResponseData = newId;
                result.ErrorCode    = newId > 0 ? ErrorCodes.Success : ErrorCodes.DatabaseError;
                result.Message      = newId > 0 ? "Student added successfully." : "Failed to add student.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentBusiness.InsertStudent: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while adding student.";
            }
            return result;
        }

        public async Task<AppResult<bool>> UpdateStudent(StudentRequestModel request)
        {
            var result = new AppResult<bool>();
            try
            {
                if (request.StudentId <= 0)
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Invalid student ID.");
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Student name is required.");

                var domain = CreateRequest<StudentDomainModel>(request);
                domain.StudentId    = request.StudentId;
                domain.Name         = request.Name;
                domain.DOB          = request.DOB;
                domain.Gender       = request.Gender;
                domain.Phone        = request.Phone;
                domain.Address      = request.Address;
                domain.DepartmentId = request.DepartmentId;

                bool updated = await _infra.UpdateStudent(domain);
                result.Status       = updated;
                result.ResponseData = updated;
                result.ErrorCode    = updated ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = updated ? "Student updated successfully." : "Student not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentBusiness.UpdateStudent: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while updating student.";
            }
            return result;
        }

        public async Task<AppResult<bool>> DeleteStudent(StudentRequestModel request)
        {
            var result = new AppResult<bool>();
            try
            {
                if (request.StudentId <= 0)
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Invalid student ID.");

                var domain = CreateRequest<StudentDomainModel>(request);
                domain.StudentId = request.StudentId;

                bool deleted = await _infra.DeleteStudent(domain);
                result.Status       = deleted;
                result.ResponseData = deleted;
                result.ErrorCode    = deleted ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = deleted ? "Student deleted successfully." : "Student not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentBusiness.DeleteStudent: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while deleting student.";
            }
            return result;
        }

        private static AppResult<T> Fail<T>(string errorCode, string message)
            => new() { Status = false, ErrorCode = errorCode, Message = message };
    }
}
