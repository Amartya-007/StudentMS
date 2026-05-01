using StudentMS.Common.Models;

namespace StudentMS.Models.ResponseModels
{
    public class DepartmentResponseModel : ResponseModelBase
    {
        public int    DepartmentId   { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool   IsActive       { get; set; }
    }
}
