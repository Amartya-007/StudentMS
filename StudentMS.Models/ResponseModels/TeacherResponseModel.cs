using StudentMS.Common.Models;

namespace StudentMS.Models.ResponseModels
{
    public class TeacherResponseModel : ResponseModelBase
    {
        public int    TeacherId      { get; set; }
        public string Name           { get; set; } = string.Empty;
        public int    DepartmentId   { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Phone          { get; set; } = string.Empty;
        public bool   IsActive       { get; set; }
    }
}
