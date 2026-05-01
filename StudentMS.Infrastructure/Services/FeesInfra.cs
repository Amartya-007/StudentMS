using Microsoft.Data.SqlClient;
using Serilog;
using StudentMS.Common.CommonInfra;
using StudentMS.Infrastructure.Interfaces;
using StudentMS.Models.DomainModels;
using StudentMS.Models.ResponseModels;

namespace StudentMS.Infrastructure.Services
{
    public class FeesInfra : IFeesInfra
    {
        public async Task<List<FeesResponseModel>> GetFees(FeesDomainModel request)
        {
            Log.Debug("FeesInfra.GetFees: Started");
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.FeesSP.GetFees);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));

                var list = new List<FeesResponseModel>();
                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);

                while (await reader.ReadAsync())
                    list.Add(MapFee(reader, ord));

                return list;
            }
            catch (Exception ex) { Log.Error(ex, "FeesInfra.GetFees: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<List<FeesResponseModel>> GetPaidFees()
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.FeesSP.GetPaidFees);
                var list = new List<FeesResponseModel>();
                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);
                while (await reader.ReadAsync()) list.Add(MapFee(reader, ord));
                return list;
            }
            catch (Exception ex) { Log.Error(ex, "FeesInfra.GetPaidFees: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<List<FeesResponseModel>> GetPendingFees()
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.FeesSP.GetPendingFees);
                var list = new List<FeesResponseModel>();
                using var reader = await dbCommand.ExecuteReaderAsync();
                var ord = Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, i => i);
                while (await reader.ReadAsync()) list.Add(MapFee(reader, ord));
                return list;
            }
            catch (Exception ex) { Log.Error(ex, "FeesInfra.GetPendingFees: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<int> InsertFee(FeesDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.FeesSP.InsertFee);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return DbHelper.CheckDbNullInt(await dbCommand.ExecuteScalarAsync());
            }
            catch (Exception ex) { Log.Error(ex, "FeesInfra.InsertFee: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        public async Task<bool> UpdateFeeStatus(FeesDomainModel request)
        {
            SqlCommand? dbCommand = null;
            try
            {
                dbCommand = await DBManager.GetStoredProcCommandAsync(DBConstants.FeesSP.UpdateFeeStatus);
                dbCommand.Parameters.AddRange(DbHelper.AddSQLParameters(request));
                return await dbCommand.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex) { Log.Error(ex, "FeesInfra.UpdateFeeStatus: Error"); throw; }
            finally { DBManager.CloseConnection(dbCommand); }
        }

        private static FeesResponseModel MapFee(SqlDataReader reader, Dictionary<string, int> ord)
            => new()
            {
                FeeId       = DbHelper.GetInt32(reader, ord, "FeeId"),
                StudentId   = DbHelper.GetInt32(reader, ord, "StudentId"),
                StudentName = DbHelper.GetString(reader, ord, "StudentName"),
                Amount      = DbHelper.GetDecimal(reader, ord, "Amount"),
                PaidDate    = DbHelper.GetDate(reader, ord, "PaidDate"),
                Status      = DbHelper.GetString(reader, ord, "Status")
            };
    }
}
