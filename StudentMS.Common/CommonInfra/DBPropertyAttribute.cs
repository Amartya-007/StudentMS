using System.Data;
using System.Runtime.Serialization;

namespace StudentMS.Common.CommonInfra
{
    /// <summary>
    /// Decorates model properties with SQL parameter metadata.
    /// Used by DbHelper.AddSQLParameters to auto-map properties to SP parameters.
    /// </summary>
    [DataContract]
    [AttributeUsage(AttributeTargets.Property)]
    public class DBPropertyAttribute : Attribute
    {
        public string?   DBElementName     { get; set; }
        public SqlDbType DBElementType     { get; set; }
        public bool      IgnoreProperty    { get; set; }
        public bool      IgnoreDefaultValue { get; set; }

        public DBPropertyAttribute(
            string?  dbelementname      = null,
            SqlDbType dbElementType     = SqlDbType.VarChar,
            bool      ignoreProperty    = false,
            bool      ignoreDefaultValue = true)
        {
            DBElementName      = dbelementname;
            DBElementType      = dbElementType;
            IgnoreProperty     = ignoreProperty;
            IgnoreDefaultValue = ignoreDefaultValue;
        }
    }
}
