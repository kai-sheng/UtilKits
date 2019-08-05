using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;

namespace UtilKits.Serializer
{
    public class FormUrlencodeHelper : ISerializer
    {
        private static Lazy<FormUrlencodeHelper> _formSerializer = new Lazy<FormUrlencodeHelper>(() => new FormUrlencodeHelper());

        /// <summary>
        /// 取得處理 Form-urlencode 序列化的執行個體
        /// </summary>
        public static FormUrlencodeHelper Instance => _formSerializer.Value;
        /// <summary>
        /// 將 Form-urlencode 字串轉為物件
        /// </summary>
        /// <param name="querystring">querystring 字串</param>
        /// <typeparam name="T">泛型類型</typeparam>
        /// <returns>泛型類型</returns>
        public T Deserialize<T>(string querystring)
        {
            var dict = HttpUtility.ParseQueryString(querystring);

            string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));

            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 將 物件轉為Form-urlencode 字串
        /// </summary>
        /// <param name="obj">要轉換的物件</param>
        /// <typeparam name="T">泛型類型</typeparam>
        /// <returns>Form-urlencode 字串</returns>
        public string Serialize<T>(T obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Select(p => $"{p.Name}={HttpUtility.UrlEncode(p.GetValue(obj, null).ToString())}");

            return string.Join("&", properties.ToArray());
        }

        /// <summary>
        /// 將 物件轉為Form-urlencode 物件
        /// </summary>
        /// <param name="obj">要轉換的物件</param>
        /// <typeparam name="T">泛型類型</typeparam>
        /// <returns>Form-urlencode 字串</returns>
        public FormUrlEncodedContent SerializeToFormUrlEncodedContent<T>(T obj)
        {
            var dict = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .ToDictionary(p => p.Name, p => HttpUtility.UrlEncode(p.GetValue(obj, null).ToString()));

            return new FormUrlEncodedContent(dict);
        }
    }
}
