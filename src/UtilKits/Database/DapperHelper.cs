using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;

namespace UtilKits.Database
{
    public class DapperHelper
    {
        public const int SQL_NO_TIMEOUT = 0;
        public const int SQL_TIMEOUT_SEC = 180;

        /// <summary>
        /// Queries the specified connection string by storeprocedure.
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters"> An array of SqlParamters used to execute the command</param>
        /// <returns></returns>
        public static T Query<T>(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            return Query<T>(connectionString, CommandType.StoredProcedure, commandText, SQL_TIMEOUT_SEC, commandParameters);
        }

        /// <summary>
        /// Queries the specified connection string.
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="timeout">The Command Timeout(default: 30s, 0: never)</param>
        /// <param name="commandParameters"> An array of SqlParamters used to execute the command</param>
        /// <returns></returns>
        public static T Query<T>(string connectionString,
            CommandType commandType, string commandText, int? timeout, params SqlParameter[] commandParameters)
        {
            return Running(connectionString, commandType, commandText, commandParameters,
                (conn, type, text, param) =>
                {
                    return conn.QuerySingleOrDefault<T>(text, param, commandType: type, commandTimeout: timeout);
                });
        }

        /// <summary>
        /// Queries the collection by storeprocedure.
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters"> An array of SqlParamters used to execute the command</param>
        /// <returns></returns>
        public static IEnumerable<T> QueryCollection<T>(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            return QueryCollection<T>(connectionString, CommandType.StoredProcedure, commandText, SQL_TIMEOUT_SEC, commandParameters);
        }

        /// <summary>
        /// Executes a query, returning the data typed as per T
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="timeout">The Command Timeout(default: 30s, 0: never)</param>
        /// <param name="commandParameters"> An array of SqlParamters used to execute the command</param>
        /// <returns>IEnumerable<T></returns>
        public static IEnumerable<T> QueryCollection<T>(string connectionString,
            CommandType commandType, string commandText, int? timeout, params SqlParameter[] commandParameters)
        {
            return Running(connectionString, commandType, commandText, commandParameters,
                (conn, type, text, param) =>
                {
                    return conn.Query<T>(text, param, commandType: type, commandTimeout: timeout);
                });
        }

        /// <summary>
        /// Queries the multiple by storeprocedure.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns></returns>
        public static void QueryMultiple(
            string connectionString, string commandText, SqlParameter[] commandParameters,
            Action<SqlMapper.GridReader> queryAction)
        {
            QueryMultiple(connectionString, CommandType.StoredProcedure, commandText, SQL_TIMEOUT_SEC, commandParameters, queryAction);
        }

        /// <summary>
        /// Execute a command that returns multiple result sets
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="timeout">The Command Timeout(default: 30s, 0: never)</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>The grid reader provides interfaces for reading multiple result sets from a Dapper query</returns>
        public static void QueryMultiple(string connectionString,
            CommandType commandType, string commandText, int? timeout, SqlParameter[] commandParameters,
            Action<SqlMapper.GridReader> queryAction)
        {
            Running(connectionString, commandType, commandText, commandParameters,
                (conn, type, text, param) =>
                {
                    var reader = conn.QueryMultiple(text, param, commandType: type, commandTimeout: timeout);

                    queryAction(reader);

                    return reader;
                });
        }

        /// <summary>
        /// Excutes the specified connection string.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns></returns>
        public static int Excute(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            return Excute(connectionString, CommandType.StoredProcedure, commandText, SQL_TIMEOUT_SEC, commandParameters);
        }

        /// <summary>
        /// Excutes the specified connection string.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="timeout">The Command Timeout(default: 30s, 0: never)</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns></returns>
        public static int Excute(string connectionString,
            CommandType commandType, string commandText, int? timeout, params SqlParameter[] commandParameters)
        {
            return Running(connectionString, commandType, commandText, commandParameters,
                (conn, type, text, param) =>
                {
                    return conn.Execute(text, param, commandType: type, commandTimeout: timeout);
                });
        }

        /// <summary>
        /// Runnings the specified connection string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// commandText
        /// </exception>
        private static T Running<T>(
            string connectionString, CommandType commandType, string commandText, SqlParameter[] commandParameters,
            Func<SqlConnection, CommandType, string, DynamicParameters, T> action)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            T result;

            using (var conn = new SqlConnection(connectionString))
            {
                DynamicParameters parameters = ParseDynamic(commandParameters);
                result = action(conn, commandType, commandText, parameters);
                SetOutputParameterValue(commandParameters, parameters);
            }

            return result;
        }

        /// <summary>
        /// Parse SqlParameter To DynamicParameters
        /// </summary>
        /// <param name="commandParameters">SqlParameter</param>
        /// <returns>DynamicParameters</returns>
        private static DynamicParameters ParseDynamic(SqlParameter[] commandParameters)
        {
            if (commandParameters == null) return null;

            DynamicParameters result = new DynamicParameters();

            foreach (SqlParameter p in commandParameters.Where(p => p != null))
            {

                if (p.Value == null)
                    p.Value = DBNull.Value;

                if (p.SqlDbType == SqlDbType.Structured && p.Value is ISqlScheme)
                {
                    var collection = p.Value as ISqlScheme;

                    if (collection.Count() > 0)
                        result.Add(p.ParameterName, new TableValueParameter(collection), p.DbType, p.Direction);
                }
                else if (p.SqlDbType == SqlDbType.Decimal)
                {
                    result.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size, p.Precision, p.Scale);
                }
                else
                {
                    result.Add(p.ParameterName, p.Value, p.DbType, p.Direction, p.Size);
                }
            }

            return result;
        }

        /// <summary>
        /// Set Output Parameter Value
        /// </summary>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <param name="dynamicParameters">A bag of parameters that can be passed to the Dapper Query and Execute methods</param>
        private static void SetOutputParameterValue(SqlParameter[] commandParameters, DynamicParameters dynamicParameters)
        {
            if (commandParameters == null) return;

            foreach (SqlParameter p in commandParameters.Where(p => p.Direction != ParameterDirection.Input))
            {
                p.Value = dynamicParameters.Get<object>(p.ParameterName);
            }

        }
    }
}
