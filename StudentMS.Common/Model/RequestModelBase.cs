namespace StudentMS.Common.Models
{
    /// <summary>Base class for all request models passed from UI to Business layer.</summary>
    public class RequestModelBase
    {
        public string? Language  { get; set; } = "en";
        public int?    UserId    { get; set; }
        public string? IPAddress { get; set; }
    }
}
