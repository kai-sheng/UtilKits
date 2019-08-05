using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace UtilKits.Database
{
    public class SqlCommandParameterHelper
    {
        /// <summary>
        /// Parses the command pararmeters.
        /// </summary>
        /// <param name="sqlCommandText">The SQL command text.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static SqlParameter[] ParseCommandPararmeters(string sqlCommandText, IEnumerable<SqlParameter> source)
        {
            List<SqlParameter> result = new List<SqlParameter>()
            {
                new SqlParameter("@stmt", SqlDbType.NVarChar) { Value = sqlCommandText },
                new SqlParameter("@parmDefinition", SqlDbType.NVarChar) { Value = ParseDefinitions(source)}
            };

            result.AddRange(source);

            return result.ToArray();
        }

        /// <summary>
        /// Parses the definitions.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        internal static string ParseDefinitions(IEnumerable<SqlParameter> source)
        {
            List<string> definitions = new List<string>();

            foreach (var parameter in source)
            {
                switch (parameter.SqlDbType)
                {
                    case SqlDbType.Int:
                        definitions.Add($"{parameter.ParameterName} INT");
                        break;
                    case SqlDbType.NVarChar:
                        definitions.Add($"{parameter.ParameterName} NVARCHAR({parameter.Size})");
                        break;
                    case SqlDbType.Structured:
                        definitions.Add($"{parameter.ParameterName} {parameter.TypeName} READONLY");
                        break;
                    default:
                        break;
                }
            }

            return string.Join(",", definitions);
        }
    }
}
