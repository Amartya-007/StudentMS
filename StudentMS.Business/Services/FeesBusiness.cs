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
    public class FeesBusiness : BusinessServiceBase, IFeesBusiness
    {
        private readonly IFeesInfra _infra;

        public FeesBusiness(IFeesInfra infra) => _infra = infra;

        public async Task<AppResult<List<FeesResponseModel>>> GetFees(FeesRequestModel request)
        {
            var result = new AppResult<List<FeesResponseModel>>();
            try
            {
                var domain = CreateRequest<FeesDomainModel>(request);
                domain.StudentId = request.StudentId;
                domain.Status    = request.Status;

                result.ResponseData = await _infra.GetFees(domain);
                result.Status       = true;
                result.ErrorCode    = ErrorCodes.Success;
                result.Message      = "Fees retrieved successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesBusiness.GetFees: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while retrieving fees.";
            }
            return result;
        }

        public async Task<AppResult<List<FeesResponseModel>>> GetPaidFees()
        {
            var result = new AppResult<List<FeesResponseModel>>();
            try
            {
                result.ResponseData = await _infra.GetPaidFees();
                result.Status       = true;
                result.ErrorCode    = ErrorCodes.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesBusiness.GetPaidFees: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred.";
            }
            return result;
        }

        public async Task<AppResult<List<FeesResponseModel>>> GetPendingFees()
        {
            var result = new AppResult<List<FeesResponseModel>>();
            try
            {
                result.ResponseData = await _infra.GetPendingFees();
                result.Status       = true;
                result.ErrorCode    = ErrorCodes.Success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesBusiness.GetPendingFees: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred.";
            }
            return result;
        }

        public async Task<AppResult<int>> InsertFee(FeesRequestModel request)
        {
            var result = new AppResult<int>();
            try
            {
                if (request.StudentId <= 0)
                    return Fail<int>(ErrorCodes.ValidationFailed, "Please select a student.");
                if (request.Amount <= 0)
                    return Fail<int>(ErrorCodes.ValidationFailed, "Amount must be greater than zero.");

                var domain = CreateRequest<FeesDomainModel>(request);
                domain.StudentId = request.StudentId;
                domain.Amount    = request.Amount;
                domain.Status    = "Pending";

                int newId = await _infra.InsertFee(domain);
                result.Status       = newId > 0;
                result.ResponseData = newId;
                result.ErrorCode    = newId > 0 ? ErrorCodes.Success : ErrorCodes.DatabaseError;
                result.Message      = newId > 0 ? "Fee added successfully." : "Failed to add fee.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesBusiness.InsertFee: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while adding fee.";
            }
            return result;
        }

        public async Task<AppResult<bool>> UpdateFeeStatus(FeesRequestModel request)
        {
            var result = new AppResult<bool>();
            try
            {
                if (request.FeeId <= 0)
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Invalid fee ID.");
                if (string.IsNullOrWhiteSpace(request.Status))
                    return Fail<bool>(ErrorCodes.ValidationFailed, "Status is required.");

                var domain = CreateRequest<FeesDomainModel>(request);
                domain.FeeId  = request.FeeId;
                domain.Status = request.Status;

                bool updated = await _infra.UpdateFeeStatus(domain);
                result.Status       = updated;
                result.ResponseData = updated;
                result.ErrorCode    = updated ? ErrorCodes.Success : ErrorCodes.NotFound;
                result.Message      = updated ? "Fee status updated successfully." : "Fee not found.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "FeesBusiness.UpdateFeeStatus: Error");
                result.Status    = false;
                result.ErrorCode = ErrorCodes.DatabaseError;
                result.Message   = "An error occurred while updating fee status.";
            }
            return result;
        }

        private static AppResult<T> Fail<T>(string errorCode, string message)
            => new() { Status = false, ErrorCode = errorCode, Message = message };
    }
}
