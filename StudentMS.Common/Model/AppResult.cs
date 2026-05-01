namespace StudentMS.Common.Models
{
    public class AppResult<T>
    {
        public bool    Status       { get; set; }
        public string? Message      { get; set; }
        public string? ErrorCode    { get; set; }
        public T?      ResponseData { get; set; }
    }

    public static class ErrorCodes
    {
        public const string Success          = "E0000";
        public const string NotFound         = "E0001";
        public const string ValidationFailed = "E0002";
        public const string DatabaseError    = "E0003";
        public const string Unauthorized     = "E0004";
    }
}
