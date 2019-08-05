using System.Collections.Generic;
using Microsoft.SqlServer.Server;

namespace UtilKits.Database
{
    public interface ISqlScheme : IEnumerable<SqlDataRecord>
    {
        string SchemeName { get; }
    }
}
