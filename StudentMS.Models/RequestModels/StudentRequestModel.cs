using StudentMS.Common.CommonInfra;
using StudentMS.Common.Models;
using System.Data;

namespace StudentMS.Models.RequestModels
{
    public class StudentRequestModel : RequestModelBase
    {
        [DBProperty("@StudentId", SqlDbType.Int)]
        public int StudentId { get; set; }

        [DBProperty("@Name", SqlDbType.NVarChar)]
        public string? Name { get; set; }

        [DBProperty("@DOB", SqlDbType.Date)]
        public DateTime? DOB { get; set; }

        [DBProperty("@Gender", SqlDbType.NVarChar)]
        public string? Gender { get; set; }

        [DBProperty("@Phone", SqlDbType.NVarChar)]
        public string? Phone { get; set; }

        [DBProperty("@Address", SqlDbType.NVarChar)]
        public string? Address { get; set; }

        [DBProperty("@DepartmentId", SqlDbType.Int)]
        public int DepartmentId { get; set; }
    }
}
