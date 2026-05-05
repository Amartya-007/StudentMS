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

        [DBProperty("@IsActive", SqlDbType.Bit)]
        public bool IsActive { get; set; } = true;

        [DBProperty("@Status", SqlDbType.NVarChar)]
        public string? Status { get; set; } = "Active";

        [DBProperty("@FailedLoginCount", SqlDbType.Int)]
        public int FailedLoginCount { get; set; }

        // ignoreDefaultValue: false so NULL is explicitly passed when LockoutUntil is null
        [DBProperty("@LockoutUntil", SqlDbType.DateTime, ignoreDefaultValue: false)]
        public DateTime? LockoutUntil { get; set; }

        /// <summary>The TeacherId to link when creating a teacher account.</summary>
        [DBProperty("@TeacherId", SqlDbType.Int)]
        public int? TeacherId { get; set; }

        /// <summary>The target UserId for admin operations (reset password, set status).</summary>
        [DBProperty("@TargetUserId", SqlDbType.Int)]
        public int? TargetUserId { get; set; }
    }
}
