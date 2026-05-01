using StudentMS.Common.CommonInfra;
using StudentMS.Common.Models;
using System.Data;

namespace StudentMS.Models.DomainModels
{
    public class UserDomainModel : DomainRequestModelBase
    {
        [DBProperty("@Username", SqlDbType.NVarChar)]
        public string? Username { get; set; }

        // BCrypt hash — set by business layer, never a plain-text password
        [DBProperty("@PasswordHash", SqlDbType.NVarChar)]
        public string? PasswordHash { get; set; }

        [DBProperty("@Role", SqlDbType.NVarChar)]
        public string? Role { get; set; } = "Staff";
    }
}
