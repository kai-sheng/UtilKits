using System;
using System.Data;
using Dapper;

namespace UtilKits.Database
{
    /// <summary>
    /// 日期一律使用UTC時間
    /// </summary>
    public class SqlTypeDateTimeUtcHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = value;
        }

        public override DateTime Parse(object value)
        {
            return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        }
    }
}
