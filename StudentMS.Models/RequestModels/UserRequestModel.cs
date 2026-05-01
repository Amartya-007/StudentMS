using StudentMS.Common.CommonInfra;
using StudentMS.Common.Models;
using System.Data;

namespace StudentMS.Models.RequestModels
{
    public class UserRequestModel : RequestModelBase
    {
        [DBProperty("@Username", SqlDbType.NVarChar)]
        public string? Username { get; set; }

        // Password is never sent as a SP parameter — BCrypt-hashed in business layer
        [DBProperty(ignoreProperty: true)]
        public string? Password { get; set; }

        [DBProperty("@Role", SqlDbType.NVarChar)]
        public string? Role { get; set; } = "Staff";
    }
}
