using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Interfaces
{
    public interface IFeesInfra
    {
        Task<List<FeesResponseModel>> GetFees(FeesDomainModel request);
        Task<List<FeesResponseModel>> GetPaidFees();
        Task<List<FeesResponseModel>> GetPendingFees();
        Task<int>                     InsertFee(FeesDomainModel request);
        Task<bool>                    UpdateFeeStatus(FeesDomainModel request);
    }
}
