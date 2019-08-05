using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.SqlServer.Server;

namespace UtilKits.Database
{
    public class TableValueParameter : SqlMapper.ICustomQueryParameter
    {
        private string _schemeName;
        private IEnumerable<SqlDataRecord> _collection;

        public TableValueParameter(ISqlScheme collection)
        {
            _schemeName = collection.SchemeName;
            _collection = collection as IEnumerable<SqlDataRecord>;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            var sqlCommand = (SqlCommand)command;

            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new SqlParameter(name, SqlDbType.Structured)
            {
                TypeName = _schemeName,
                Value = _collection
            });
        }
    }
}