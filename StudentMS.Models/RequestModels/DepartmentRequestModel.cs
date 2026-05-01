using StudentMS.Common.CommonInfra;
using StudentMS.Common.Models;
using System.Data;

namespace StudentMS.Models.RequestModels
{
    public class DepartmentRequestModel : RequestModelBase
    {
        [DBProperty("@DepartmentId", SqlDbType.Int)]
        public int DepartmentId { get; set; }

        [DBProperty("@DepartmentName", SqlDbType.NVarChar)]
        public string? DepartmentName { get; set; }
    }
}
