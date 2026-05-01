using StudentMS.Common.CommonInfra;
using StudentMS.Common.Models;
using System.Data;

namespace StudentMS.Models.DomainModels
{
    public class TeacherDomainModel : DomainRequestModelBase
    {
        [DBProperty("@TeacherId", SqlDbType.Int)]
        public int TeacherId { get; set; }

        [DBProperty("@Name", SqlDbType.NVarChar)]
        public string? Name { get; set; }

        [DBProperty("@DepartmentId", SqlDbType.Int)]
        public int DepartmentId { get; set; }

        [DBProperty("@Phone", SqlDbType.NVarChar)]
        public string? Phone { get; set; }
    }
}
