using StudentMS.Common.CommonInfra;
using StudentMS.Common.Models;
using System.Data;

namespace StudentMS.Models.DomainModels
{
    public class DepartmentDomainModel : DomainRequestModelBase
    {
        [DBProperty("@DepartmentId", SqlDbType.Int)]
        public int DepartmentId { get; set; }

        [DBProperty("@DepartmentName", SqlDbType.NVarChar)]
        public string? DepartmentName { get; set; }
    }
}
