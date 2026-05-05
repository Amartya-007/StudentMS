using Serilog;
using StudentMS.Business.Base;
using StudentMS.Business.Interfaces;
using StudentMS.Common.Logging;
using StudentMS.Common.Models;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Models.DomainModels;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Business.Services
{
    public class TeacherBusiness : BusinessServiceBase, ITeacherBusiness
    {
        private readonly ITeacherInfra _infra;
        private readonly IActivityLogBusiness _logger;

        public TeacherBusiness(ITeacherInfra infra, IActivityLogBusiness logger)
        {
            _infra  = infra;
            _logger = logger;
        }

        public async Task<AppResult<List<TeacherResponseModel>>> GetTeachers(TeacherRequestModel request)
        {
            var result = new AppResult<List<TeacherResponseModel>>();
            try
            {
                var domain = CreateRequest<TeacherDomainModel>(request);
                domain.DepartmentId = request.DepartmentId;

                result.ResponseData = await _infra.GetTeachers(domain);
                result.Status       = true;
                result.ErrorCode    = ErrorCodes.Success;
                result.Message      = "Teachers retrieved successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherBusiness.GetTeachers: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while retrieving teachers.";
            }
            return result;
        }

        public async Task<AppResult<TeacherResponseModel>> GetTeacherById(TeacherRequestModel request)
        {
            var result = new AppResult<TeacherResponseModel>();
            try
            {
                var domain = CreateRequest<TeacherDomainModel>(request);
                domain.TeacherId = request.TeacherId;

                var data = await _infra.GetTeacherById(domain);
                result.Status       = data != null;
                result.ResponseData = data;
                result.ErrorCode    = data != null ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = data != null ? "Teacher found." : "Teacher not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherBusiness.GetTeacherById: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred.";
            }
            return result;
        }

        public async Task<AppResult<int>> InsertTeacher(TeacherRequestModel request)
        {
            var result = new AppResult<int>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Fail<int>(ErrorCodes.ValidationFailed, "Teacher name is required.");
                if (request.DepartmentId <= 0)
                    return Fail<int>(ErrorCodes.ValidationFailed, "Please select a department.");

                var domain = CreateRequest<TeacherDomainModel>(request);
                domain.Name         = request.Name;
                domain.Phone        = request.Phone;
                domain.DepartmentId = request.DepartmentId;

                int newId = await _infra.InsertTeacher(domain);
                result.Status       = newId > 0;
                result.ResponseData = newId;
                result.ErrorCode    = newId > 0 ? ErrorCodes.Success : ErrorCodes.DatabaseError;
                result.Message      = newId > 0 ? "Teacher added successfully." : "Failed to add teacher.";

                if (newId > 0)
                {
                    _logger.LogActivity(ActivityActions.TeacherCreate, $"Created teacher '{request.Name}'", "Teacher", newId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherBusiness.InsertTeacher: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while adding teacher.";
            }
            return result;
        }

        public async Task<AppResult<bool>> UpdateTeacher(TeacherRequestModel request)
        {
            var result = new AppResult<bool>();
            try
            {
                if (request.TeacherId <= 0)
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Invalid teacher ID.");
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Teacher name is required.");

                // Fetch existing teacher before update for audit log
                var existingDomain = CreateRequest<TeacherDomainModel>(request);
                existingDomain.TeacherId = request.TeacherId;
                var existingTeacher = await _infra.GetTeacherById(existingDomain);

                var domain = CreateRequest<TeacherDomainModel>(request);
                domain.TeacherId    = request.TeacherId;
                domain.Name         = request.Name;
                domain.Phone        = request.Phone;
                domain.DepartmentId = request.DepartmentId;

                bool updated = await _infra.UpdateTeacher(domain);
                result.Status       = updated;
                result.ResponseData = updated;
                result.ErrorCode    = updated ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = updated ? "Teacher updated successfully." : "Teacher not found.";

                if (updated)
                {
                    _logger.LogActivityWithAudit(ActivityActions.TeacherUpdate,
                        $"Updated teacher '{request.Name}'", "Teacher", request.TeacherId,
                        existingTeacher, domain);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherBusiness.UpdateTeacher: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while updating teacher.";
            }
            return result;
        }

        public async Task<AppResult<bool>> DeleteTeacher(TeacherRequestModel request)
        {
            var result = new AppResult<bool>();
            try
            {
                if (request.TeacherId <= 0)
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Invalid teacher ID.");

                var domain = CreateRequest<TeacherDomainModel>(request);
                domain.TeacherId = request.TeacherId;

                bool deleted = await _infra.DeleteTeacher(domain);
                result.Status       = deleted;
                result.ResponseData = deleted;
                result.ErrorCode    = deleted ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = deleted ? "Teacher deleted successfully." : "Teacher not found.";

                if (deleted)
                {
                    _logger.LogActivity(ActivityActions.TeacherDelete, $"Deleted teacher id={request.TeacherId}", "Teacher", request.TeacherId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "TeacherBusiness.DeleteTeacher: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while deleting teacher.";
            }
            return result;
        }

        private static AppResult<T> Fail<T>(string errorCode, string message)
            => new() { Status = false, ErrorCode = errorCode, Message = message };
    }
}
