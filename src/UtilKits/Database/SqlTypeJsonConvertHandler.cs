using System;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace UtilKits.Database
{
    /// <summary>
    /// 設定當資料庫存取為字串，但 Model 類型為泛型或清單時，可轉換為該類型
    /// ex. varchar [1,2,3] to int of List
    /// ex. nvarchar ["Crab", "Eric", "Hitomi"] to stirng of List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlTypeJsonConvertHandler<T> : SqlMapper.TypeHandler<T>
    {
        public override T Parse(object value)
        {
            return JsonConvert.DeserializeObject<T>((string) value);
        }

        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = (value == null) ? (object) DBNull.Value : JsonConvert.SerializeObject(value);
            parameter.DbType = DbType.String;
        }
    }
}