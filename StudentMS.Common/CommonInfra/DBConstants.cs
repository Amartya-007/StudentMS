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
            public const string GetUserByUsername    = "dbo.USP_GetUserByUsername";
            public const string CreateUser           = "dbo.USP_CreateUser";
            public const string UpdateFailedLogin    = "dbo.USP_UpdateFailedLoginCount";
            public const string SetLockout           = "dbo.USP_SetLockout";
            public const string ResetLockout         = "dbo.USP_ResetLockout";
            public const string UpdatePasswordHash   = "dbo.USP_UpdatePasswordHash";
            public const string GetAllUsers          = "dbo.USP_GetAllUsers";
            public const string SetUserActiveStatus  = "dbo.USP_SetUserActiveStatus";
            public const string CheckUsernameExists  = "dbo.USP_CheckUsernameExists";
            public const string GetUserById          = "dbo.USP_GetUserById";
            public const string LinkUserToTeacher    = "dbo.USP_LinkUserToTeacher";
        }

        public struct ActivityLogSP
        {
            public const string InsertActivityLog  = "dbo.USP_InsertActivityLog";
            public const string InsertAuditLog     = "dbo.USP_InsertAuditLog";
            public const string GetActivityLogs    = "dbo.USP_GetActivityLogs";
            public const string GetAuditLogByLogId = "dbo.USP_GetAuditLogByLogId";
        }
    }
}
