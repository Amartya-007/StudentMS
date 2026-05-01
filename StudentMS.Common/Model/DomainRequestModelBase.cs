namespace StudentMS.Common.Models
{
    /// <summary>Base class for all domain models passed from Business to Infrastructure layer.</summary>
    public class DomainRequestModelBase
    {
        public string? Language  { get; set; } = "en";
        public int?    UserId    { get; set; }
        public string? IPAddress { get; set; }
    }
}
