using StudentMS.Common.Models;

namespace StudentMS.Models.ResponseModels
{
    public class UserResponseModel : ResponseModelBase
    {
        public int       UserId           { get; set; }
        public string    Username         { get; set; } = string.Empty;
        public string    Role             { get; set; } = string.Empty;  // Admin / Teacher
        public bool      IsActive         { get; set; }
        public string    Status           { get; set; } = "Active";
        public int       FailedLoginCount { get; set; }
        public DateTime? LockoutUntil     { get; set; }
        public int?      TeacherId        { get; set; }
        public DateTime  CreatedOn        { get; set; }
    }
}
