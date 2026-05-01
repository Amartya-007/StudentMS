using StudentMS.Common.Models;

namespace StudentMS.Models.ResponseModels
{
    public class FeesResponseModel : ResponseModelBase
    {
        public int       FeeId       { get; set; }
        public int       StudentId   { get; set; }
        public string    StudentName { get; set; } = string.Empty;
        public decimal   Amount      { get; set; }
        public DateTime? PaidDate    { get; set; }
        public string    Status      { get; set; } = string.Empty;  // Paid / Pending
    }
}
