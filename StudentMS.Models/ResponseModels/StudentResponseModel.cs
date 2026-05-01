using StudentMS.Common.Models;

namespace StudentMS.Models.ResponseModels
{
    public class StudentResponseModel : ResponseModelBase
    {
        public int       StudentId      { get; set; }
        public string    Name           { get; set; } = string.Empty;
        public DateTime? DOB            { get; set; }
        public string    Gender         { get; set; } = string.Empty;
        public string    Phone          { get; set; } = string.Empty;
        public string    Address        { get; set; } = string.Empty;
        public int       DepartmentId   { get; set; }
        public string    DepartmentName { get; set; } = string.Empty;
        public bool      IsActive       { get; set; }
    }
}
