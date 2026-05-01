using StudentMS.Common.Models;

namespace StudentMS.Models.ResponseModels
{
    public class UserResponseModel : ResponseModelBase
    {
        public int    UserId   { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role     { get; set; } = string.Empty;  // Admin / Staff
        public bool   IsActive { get; set; }
    }
}
