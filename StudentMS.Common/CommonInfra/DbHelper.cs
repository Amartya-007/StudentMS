using Microsoft.Data.SqlClient;
using StudentMS.Common.Models;
using System.Data;
using System.Reflection;

namespace StudentMS.Common.CommonInfra
{
    /// <summary>
    /// Utility class for safe DB null checks, SQL parameter building, and safe DataReader helpers.
    /// </summary>
    public static class DbHelper
    {
        // ── Null-safe scalar readers ─────────────────────────────────────────────

        public static int CheckDbNullInt(object? obj)
            => obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);

        public static long CheckDbNullLong(object? obj)
            => obj == null || obj == DBNull.Value ? 0L : Convert.ToInt64(obj);

        public static short CheckDbNullShort(object? obj)
            => obj == null || obj == DBNull.Value ? (short)0 : Convert.ToInt16(obj);

        public static string CheckDbNullString(object? obj)
            => obj == null || obj == DBNull.Value ? string.Empty : Convert.ToString(obj) ?? string.Empty;

        public static DateTime? CheckDbNullDate(object? obj)
            => obj == null || obj == DBNull.Value ? null : Convert.ToDateTime(obj);

        public static bool CheckDbNullBool(object? obj)
            => obj != null && obj != DBNull.Value && Convert.ToBoolean(obj);

        public static decimal CheckDbNullDecimal(object? obj)
            => obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);

        public static float CheckDbNullFloat(object? obj)
            => obj == null || obj == DBNull.Value ? 0f : Convert.ToSingle(obj);

        public static double CheckDbNullDouble(object? obj)
            => obj == null || obj == DBNull.Value ? 0d : Convert.ToDouble(obj);

        public static Guid CheckDbNullGuid(object? obj)
            => obj == null || obj == DBNull.Value ? Guid.Empty : Guid.Parse(obj.ToString()!);

        public static byte[]? CheckDbNullByteArray(object? obj)
            => obj == null || obj == DBNull.Value ? null : (byte[])obj;

        // ── Safe DataReader helpers (ordinal-based for performance) ──────────────

        public static int GetInt32(SqlDataReader reader, Dictionary<string, int> ordinals, string column)
            => ordinals.TryGetValue(column, out int i) ? CheckDbNullInt(reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i)) : 0;

        public static long GetInt64(SqlDataReader reader, Dictionary<string, int> ordinals, string column)
            => ordinals.TryGetValue(column, out int i) ? CheckDbNullLong(reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i)) : 0L;

        public static short GetInt16(SqlDataReader reader, Dictionary<string, int> ordinals, string column)
            => ordinals.TryGetValue(column, out int i) ? CheckDbNullShort(reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i)) : (short)0;

        public static string GetString(SqlDataReader reader, Dictionary<string, int> ordinals, string column)
            => ordinals.TryGetValue(column, out int i) ? CheckDbNullString(reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i)) : string.Empty;

        public static bool GetBool(SqlDataReader reader, Dictionary<string, int> ordinals, string column)
            => ordinals.TryGetValue(column, out int i) && CheckDbNullBool(reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i));

        public static decimal GetDecimal(SqlDataReader reader, Dictionary<string, int> ordinals, string column)
            => ordinals.TryGetValue(column, out int i) ? CheckDbNullDecimal(reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i)) : 0m;

        public static DateTime? GetDate(SqlDataReader reader, Dictionary<string, int> ordinals, string column)
            => ordinals.TryGetValue(column, out int i) ? CheckDbNullDate(reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i)) : null;

        // ── SqlDbType mapping ────────────────────────────────────────────────────

        public static SqlDbType GetDBType(string type) => type.ToLower() switch
        {
            "int"      => SqlDbType.Int,
            "bigint"   => SqlDbType.BigInt,
            "smallint" => SqlDbType.SmallInt,
            "bit"      => SqlDbType.Bit,
            "decimal"  => SqlDbType.Decimal,
            "float"    => SqlDbType.Float,
            "datetime" => SqlDbType.DateTime,
            "date"     => SqlDbType.Date,
            "uniqueidentifier" => SqlDbType.UniqueIdentifier,
            "varbinary" => SqlDbType.VarBinary,
            _          => SqlDbType.VarChar
        };

        // ── Parameter builder ────────────────────────────────────────────────────

        public static SqlParameter AddParameters(SqlDbType dbType, string paramName, object? value, bool ignoreDefaultValue = true)
        {
            var param = new SqlParameter(paramName, dbType);

            if (value == null || (ignoreDefaultValue && IsDefaultValue(value)))
                param.Value = DBNull.Value;
            else
                param.Value = value;

            return param;
        }

        private static bool IsDefaultValue(object value)
        {
            if (value is int    i && i == 0)    return true;
            if (value is long   l && l == 0)    return true;
            if (value is short  s && s == 0)    return true;
            if (value is decimal d && d == 0)   return true;
            if (value is float  f && f == 0)    return true;
            if (value is double dbl && dbl == 0) return true;
            if (value is Guid   g && g == Guid.Empty) return true;
            if (value is string str && string.IsNullOrEmpty(str)) return true;
            return false;
        }

        /// <summary>
        /// Reflects over a DomainRequestModelBase-derived object and builds SqlParameter[]
        /// from all properties decorated with [DBProperty], skipping IgnoreProperty ones.
        /// </summary>
        public static SqlParameter[] AddSQLParameters<T>(T model) where T : DomainRequestModelBase
        {
            var parameters = new List<SqlParameter>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<DBPropertyAttribute>();
                if (attr == null || attr.IgnoreProperty) continue;

                string paramName = attr.DBElementName ?? $"@{prop.Name}";
                object? value    = prop.GetValue(model);

                var param = new SqlParameter(paramName, attr.DBElementType);

                if (value == null || (attr.IgnoreDefaultValue && IsDefaultValue(value)))
                    param.Value = DBNull.Value;
                else
                    param.Value = value;

                parameters.Add(param);
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Converts a List&lt;T&gt; to a DataTable using public properties.
        /// </summary>
        public static DataTable ConvertToDataTable<T>(List<T> list)
        {
            var table = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in props)
                table.Columns.Add(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);

            foreach (var item in list)
            {
                var row = table.NewRow();
                foreach (var p in props)
                    row[p.Name] = p.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
