using Microsoft.Data.SqlClient;
using Serilog;
using StudentMS.Common.CommonInfra;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Services
{
    public class TeacherInfra : ITeacherInfra
    {
        public async Task<List<TeacherResponseModel>> GetTeachers(TeacherDomainModel request)
        {
            Log.Debug("TeacherInfra.GetTeachers: Started");
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.TeacherSP.GetTeachers);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));

                var list = new List<TeacherResponseModel>();
                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);

                while (await reader.ReadAsync())
                    list.Add(MapTeacher(reader, ord));

                Log.Debug("TeacherInfra.GetTeachers: Returned {Count} records", list.Count);
                return list;
            }
            catch (Exception ex) { Log.Error(ex, "TeacherInfra.GetTeachers: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<TeacherResponseModel?> GetTeacherById(TeacherDomainModel request)
        {
            Log.Debug("TeacherInfra.GetTeacherById: TeacherId={Id}", request.TeacherId);
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.TeacherSP.GetTeacherById);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));

                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);

                return await reader.ReadAsync() ? MapTeacher(reader, ord) : null;
            }
            catch (Exception ex) { Log.Error(ex, "TeacherInfra.GetTeacherById: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<int> InsertTeacher(TeacherDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.TeacherSP.InsertTeacher);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return DbHelper.CheckDbNullInt(await dbCommand.ExecuteScalarAsync());
            }
            catch (Exception ex) { Log.Error(ex, "TeacherInfra.InsertTeacher: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<bool> UpdateTeacher(TeacherDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.TeacherSP.UpdateTeacher);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return await dbCommand.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex) { Log.Error(ex, "TeacherInfra.UpdateTeacher: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<bool> DeleteTeacher(TeacherDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.TeacherSP.DeleteTeacher);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return await dbCommand.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex) { Log.Error(ex, "TeacherInfra.DeleteTeacher: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        private static TeacherResponseModel MapTeacher(SqlDataReader reader, Dictionary<string, int> ord)
            => new()
            {
                TeacherId      = DbHelper.GetInt32(reader, ord, "TeacherId"),
                Name           = DbHelper.GetString(reader, ord, "Name"),
                DepartmentId   = DbHelper.GetInt32(reader, ord, "DepartmentId"),
                DepartmentName = DbHelper.GetString(reader, ord, "DepartmentName"),
                Phone          = DbHelper.GetString(reader, ord, "Phone"),
                IsActive       = DbHelper.GetBool(reader, ord, "IsActive")
            };
    }
}
