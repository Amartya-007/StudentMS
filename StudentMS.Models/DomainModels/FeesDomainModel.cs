using StudentMS.Common.CommonInfra;
using StudentMS.Common.Models;
using System.Data;

namespace StudentMS.Models.DomainModels
{
    public class FeesDomainModel : DomainRequestModelBase
    {
        [DBProperty("@FeeId", SqlDbType.Int)]
        public int FeeId { get; set; }

        [DBProperty("@StudentId", SqlDbType.Int)]
        public int StudentId { get; set; }

        [DBProperty("@Amount", SqlDbType.Decimal)]
        public decimal Amount { get; set; }

        [DBProperty("@Status", SqlDbType.NVarChar)]
        public string? Status { get; set; }
    }
}
