using Microsoft.Data.SqlClient;
using Serilog;
using StudentMS.Common.CommonInfra;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Services
{
    public class StudentInfra : IStudentInfra
    {
        public async Task<List<StudentResponseModel>> GetStudents(StudentDomainModel request)
        {
            Log.Debug("StudentInfra.GetStudents: Started");
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.StudentSP.GetStudents);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));

                var list = new List<StudentResponseModel>();
                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = BuildOrdinals(reader);

                while (await reader.ReadAsync())
                {
                    list.Add(MapStudent(reader, ord));
                }

                Log.Debug("StudentInfra.GetStudents: Returned {Count} records", list.Count);
                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentInfra.GetStudents: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        public async Task<StudentResponseModel?> GetStudentById(StudentDomainModel request)
        {
            Log.Debug("StudentInfra.GetStudentById: StudentId={Id}", request.StudentId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.StudentSP.GetStudentById);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));

                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = BuildOrdinals(reader);

                if (await reader.ReadAsync())
                    return MapStudent(reader, ord);

                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentInfra.GetStudentById: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        public async Task<int> InsertStudent(StudentDomainModel request)
        {
            Log.Debug("StudentInfra.InsertStudent: Started");
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.StudentSP.InsertStudent);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                var result = await dbCommand.ExecuteScalarAsync();
                return DbHelper.CheckDbNullInt(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentInfra.InsertStudent: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        public async Task<bool> UpdateStudent(StudentDomainModel request)
        {
            Log.Debug("StudentInfra.UpdateStudent: StudentId={Id}", request.StudentId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.StudentSP.UpdateStudent);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                int rows = await dbCommand.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentInfra.UpdateStudent: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        public async Task<bool> DeleteStudent(StudentDomainModel request)
        {
            Log.Debug("StudentInfra.DeleteStudent: StudentId={Id}", request.StudentId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.StudentSP.DeleteStudent);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                int rows = await dbCommand.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "StudentInfra.DeleteStudent: Error");
                throw;
            }
            finally
            {
                DBManager.CloseConnection(dbCommand);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static Dictionary<string, int> BuildOrdinals(SqlDataReader reader)
            => Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);

        private static StudentResponseModel MapStudent(SqlDataReader reader, Dictionary<string, int> ord)
            => new()
            {
                StudentId      = DbHelper.GetInt32(reader, ord, "StudentId"),
                Name           = DbHelper.GetString(reader, ord, "Name"),
                DOB            = DbHelper.GetDate(reader, ord, "DOB"),
                Gender         = DbHelper.GetString(reader, ord, "Gender"),
                Phone          = DbHelper.GetString(reader, ord, "Phone"),
                Address        = DbHelper.GetString(reader, ord, "Address"),
                DepartmentId   = DbHelper.GetInt32(reader, ord, "DepartmentId"),
                DepartmentName = DbHelper.GetString(reader, ord, "DepartmentName"),
                IsActive       = DbHelper.GetBool(reader, ord, "IsActive")
            };
    }
}
