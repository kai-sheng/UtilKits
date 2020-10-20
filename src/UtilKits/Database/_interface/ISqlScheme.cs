using System.Collections.Generic;
using Microsoft.SqlServer.Server;

namespace UtilKits.Database
{
    /// <summary>
    /// 使用 Table Scheme 需實作該介面
    /// DapperHelper 方可轉換自定義 Table
    /// </summary>
    public interface ISqlScheme : IEnumerable<SqlDataRecord>
    {
        string SchemeName { get; }
    }
}
