using StudentMS.Common.Models;
using StudentMS.Models.RequestModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Business.Interfaces
{
    public interface IFeesBusiness
    {
        Task<AppResult<List<FeesResponseModel>>> GetFees(FeesRequestModel request);
        Task<AppResult<List<FeesResponseModel>>> GetPaidFees();
        Task<AppResult<List<FeesResponseModel>>> GetPendingFees();
        Task<AppResult<int>>                     InsertFee(FeesRequestModel request);
        Task<AppResult<bool>>                    UpdateFeeStatus(FeesRequestModel request);
    }
}
