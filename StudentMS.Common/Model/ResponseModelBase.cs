namespace StudentMS.Common.Models
{
    /// <summary>Base class for all response models returned from Infrastructure layer.</summary>
    public class ResponseModelBase
    {
        public int? TotalCount { get; set; }
    }
}
