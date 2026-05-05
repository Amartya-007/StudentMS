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
    public class DepartmentBusiness : BusinessServiceBase, IDepartmentBusiness
    {
        private readonly IDepartmentInfra _infra;
        private readonly IActivityLogBusiness _logger;

        public DepartmentBusiness(IDepartmentInfra infra, IActivityLogBusiness logger)
        {
            _infra  = infra;
            _logger = logger;
        }

        public async Task<AppResult<List<DepartmentResponseModel>>> GetDepartments(DepartmentRequestModel request)
        {
            var result = new AppResult<List<DepartmentResponseModel>>();
            try
            {
                var domain = CreateRequest<DepartmentDomainModel>(request);
                result.ResponseData = await _infra.GetDepartments(domain);
                result.Status       = true;
                result.ErrorCode    = ErrorCodes.Success;
                result.Message      = "Departments retrieved successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentBusiness.GetDepartments: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while retrieving departments.";
            }
            return result;
        }

        public async Task<AppResult<DepartmentResponseModel>> GetDepartmentById(DepartmentRequestModel request)
        {
            var result = new AppResult<DepartmentResponseModel>();
            try
            {
                var domain = CreateRequest<DepartmentDomainModel>(request);
                domain.DepartmentId = request.DepartmentId;

                var data = await _infra.GetDepartmentById(domain);
                result.Status       = data != null;
                result.ResponseData = data;
                result.ErrorCode    = data != null ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = data != null ? "Department found." : "Department not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentBusiness.GetDepartmentById: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred.";
            }
            return result;
        }

        public async Task<AppResult<int>> InsertDepartment(DepartmentRequestModel request)
        {
            var result = new AppResult<int>();
            try
            {
                if (string.IsNullOrWhiteSpace(request.DepartmentName))
                    return Fail<int>(ErrorCodes.ValidationFailed, "Department name is required.");

                var domain = CreateRequest<DepartmentDomainModel>(request);
                domain.DepartmentName = request.DepartmentName;

                int newId = await _infra.InsertDepartment(domain);
                result.Status       = newId > 0;
                result.ResponseData = newId;
                result.ErrorCode    = newId > 0 ? ErrorCodes.Success : ErrorCodes.DatabaseError;
                result.Message      = newId > 0 ? "Department added successfully." : "Failed to add department.";

                if (newId > 0)
                {
                    _logger.LogActivity(ActivityActions.DepartmentCreate, $"Created department '{request.DepartmentName}'", "Department", newId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentBusiness.InsertDepartment: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while adding department.";
            }
            return result;
        }

        public async Task<AppResult<bool>> UpdateDepartment(DepartmentRequestModel request)
        {
            var result = new AppResult<bool>();
            try
            {
                if (request.DepartmentId <= 0)
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Invalid department ID.");
                if (string.IsNullOrWhiteSpace(request.DepartmentName))
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Department name is required.");

                // Fetch existing department before update for audit log
                var existingDomain = CreateRequest<DepartmentDomainModel>(request);
                existingDomain.DepartmentId = request.DepartmentId;
                var existingDept = await _infra.GetDepartmentById(existingDomain);

                var domain = CreateRequest<DepartmentDomainModel>(request);
                domain.DepartmentId   = request.DepartmentId;
                domain.DepartmentName = request.DepartmentName;

                bool updated = await _infra.UpdateDepartment(domain);
                result.Status       = updated;
                result.ResponseData = updated;
                result.ErrorCode    = updated ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = updated ? "Department updated successfully." : "Department not found.";

                if (updated)
                {
                    _logger.LogActivityWithAudit(ActivityActions.DepartmentUpdate,
                        $"Updated department '{request.DepartmentName}'", "Department", request.DepartmentId,
                        existingDept, domain);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentBusiness.UpdateDepartment: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while updating department.";
            }
            return result;
        }

        public async Task<AppResult<bool>> DeleteDepartment(DepartmentRequestModel request)
        {
            var result = new AppResult<bool>();
            try
            {
                if (request.DepartmentId <= 0)
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Invalid department ID.");

                var domain = CreateRequest<DepartmentDomainModel>(request);
                domain.DepartmentId = request.DepartmentId;

                bool deleted = await _infra.DeleteDepartment(domain);
                result.Status       = deleted;
                result.ResponseData = deleted;
                result.ErrorCode    = deleted ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = deleted ? "Department deleted successfully." : "Department not found.";

                if (deleted)
                {
                    _logger.LogActivity(ActivityActions.DepartmentDelete, $"Deleted department id={request.DepartmentId}", "Department", request.DepartmentId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DepartmentBusiness.DeleteDepartment: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while deleting department.";
            }
            return result;
        }

        private static AppResult<T> Fail<T>(string errorCode, string message)
            => new() { Status = false, ErrorCode = errorCode, Message = message };
    }
}
