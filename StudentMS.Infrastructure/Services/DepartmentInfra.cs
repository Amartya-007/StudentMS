using Microsoft.Data.SqlClient;
using Serilog;
using StudentMS.Common.CommonInfra;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Services
{
    public class DepartmentInfra : IDepartmentInfra
    {
        public async Task<List<DepartmentResponseModel>> GetDepartments(DepartmentDomainModel request)
        {
            Log.Debug("DepartmentInfra.GetDepartments: Started");
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.DepartmentSP.GetDepartments);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));

                var list = new List<DepartmentResponseModel>();
                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);

                while (await reader.ReadAsync())
                    list.Add(MapDepartment(reader, ord));

                Log.Debug("DepartmentInfra.GetDepartments: Returned {Count} records", list.Count);
                return list;
            }
            catch (Exception ex) { Log.Error(ex, "DepartmentInfra.GetDepartments: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<DepartmentResponseModel?> GetDepartmentById(DepartmentDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.DepartmentSP.GetDepartmentById);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));

                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);

                return await reader.ReadAsync() ? MapDepartment(reader, ord) : null;
            }
            catch (Exception ex) { Log.Error(ex, "DepartmentInfra.GetDepartmentById: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<int> InsertDepartment(DepartmentDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.DepartmentSP.InsertDepartment);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return DbHelper.CheckDbNullInt(await dbCommand.ExecuteScalarAsync());
            }
            catch (Exception ex) { Log.Error(ex, "DepartmentInfra.InsertDepartment: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<bool> UpdateDepartment(DepartmentDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.DepartmentSP.UpdateDepartment);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return await dbCommand.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex) { Log.Error(ex, "DepartmentInfra.UpdateDepartment: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<bool> DeleteDepartment(DepartmentDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.DepartmentSP.DeleteDepartment);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return await dbCommand.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex) { Log.Error(ex, "DepartmentInfra.DeleteDepartment: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        private static DepartmentResponseModel MapDepartment(SqlDataReader reader, Dictionary<string, int> ord)
            => new()
            {
                DepartmentId   = DbHelper.GetInt32(reader, ord, "DepartmentId"),
                DepartmentName = DbHelper.GetString(reader, ord, "DepartmentName"),
                IsActive       = DbHelper.GetBool(reader, ord, "IsActive")
            };
    }
}
