using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UtilKits.Serializer
{
    public class JsonHelper : ISerializer
    {
        private static Lazy<JsonHelper> _jsonSerializer = new Lazy<JsonHelper>(() => new JsonHelper());

        private static Lazy<JsonSerializerSettings> _jsonSerialSetting = new Lazy<JsonSerializerSettings>(() => new JsonSerializerSettings()
        {
            ContractResolver = new IncludeNonPublicMembersContractResolver()
        });

        /// <summary>
        /// 取得處理Json序列化的執行個體
        /// </summary>
        public static JsonHelper Instance => _jsonSerializer.Value;
        /// <summary>
        /// 序列化設定, 連同 private 變數一同序列化
        /// </summary>
        protected static JsonSerializerSettings settings => _jsonSerialSetting.Value;

        /// <summary>
        /// 將物件轉換為Json字串
        /// </summary>
        /// <param name="obj">要轉換的物件</param>
        /// <returns>Json 字串</returns>
        public string Serialize<T>(T obj)
        {
            string rtnString = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return rtnString;
        }

        /// <summary>
        /// 將物件轉換為Json字串(連同 private 變數一同序列化)
        /// </summary>
        /// <param name="obj">要轉換的物件</param>
        /// <returns>Json 字串</returns>
        public string SerializeAllMembers<T>(T obj)
        {
            string rtnString = JsonConvert.SerializeObject(obj, settings);
            return rtnString;
        }

        /// <summary>
        /// 將Json字串轉換為泛型物件
        /// </summary>
        /// <typeparam name="T">泛型類型</typeparam>
        /// <param name="jsonString">Json 字串</param>
        /// <returns>泛型物件</returns>
        public T Deserialize<T>(string jsonString)
        {
            T rtnValue = JsonConvert.DeserializeObject<T>(jsonString);
            return rtnValue;
        }

        /// <summary>
        /// 將Json字串轉換為泛型物件(連同 private 變數一同反序列化)
        /// </summary>
        /// <typeparam name="T">泛型類型</typeparam>
        /// <param name="jsonString">Json 字串</param>
        /// <returns>泛型物件</returns>
        public T DeserializeAllMembers<T>(string jsonString)
        {
            T rtnValue = JsonConvert.DeserializeObject<T>(jsonString, settings);
            return rtnValue;
        }

        private class IncludeNonPublicMembersContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Select(p => base.CreateProperty(p, memberSerialization))
                            .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Select(f => base.CreateProperty(f, memberSerialization)))
                            .ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props;
            }
        }
    }
}
