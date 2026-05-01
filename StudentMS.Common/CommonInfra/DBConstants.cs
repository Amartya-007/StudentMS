namespace StudentMS.Common.CommonInfra
{
    public static class DBConstants
    {
        public struct StudentSP
        {
            public const string GetStudents    = "dbo.USP_GetStudents";
            public const string GetStudentById = "dbo.USP_GetStudentById";
            public const string InsertStudent  = "dbo.USP_InsertStudent";
            public const string UpdateStudent  = "dbo.USP_UpdateStudent";
            public const string DeleteStudent  = "dbo.USP_DeleteStudent";
        }

        public struct DepartmentSP
        {
            public const string GetDepartments    = "dbo.USP_GetDepartments";
            public const string GetDepartmentById = "dbo.USP_GetDepartmentById";
            public const string InsertDepartment  = "dbo.USP_InsertDepartment";
            public const string UpdateDepartment  = "dbo.USP_UpdateDepartment";
            public const string DeleteDepartment  = "dbo.USP_DeleteDepartment";
        }

        public struct TeacherSP
        {
            public const string GetTeachers    = "dbo.USP_GetTeachers";
            public const string GetTeacherById = "dbo.USP_GetTeacherById";
            public const string InsertTeacher  = "dbo.USP_InsertTeacher";
            public const string UpdateTeacher  = "dbo.USP_UpdateTeacher";
            public const string DeleteTeacher  = "dbo.USP_DeleteTeacher";
        }

        public struct FeesSP
        {
            public const string GetFees        = "dbo.USP_GetFees";
            public const string GetPaidFees    = "dbo.USP_GetPaidFees";
            public const string GetPendingFees = "dbo.USP_GetPendingFees";
            public const string InsertFee      = "dbo.USP_InsertFee";
            public const string UpdateFeeStatus = "dbo.USP_UpdateFeeStatus";
        }

        public struct UserSP
        {
            public const string GetUserByUsername = "dbo.USP_GetUserByUsername";
            public const string CreateUser        = "dbo.USP_CreateUser";
        }
    }
}
