using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace UtilKits.Serializer
{
    public class MultipartFormHelper : ISerializer
    {
        private const string AlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static Lazy<MultipartFormHelper> _formSerializer = new Lazy<MultipartFormHelper>(() => new MultipartFormHelper());

        /// <summary>
        /// 取得處理 Multipart Form 序列化的執行個體
        /// </summary>
        public static MultipartFormHelper Instance => _formSerializer.Value;

        public T Deserialize<T>(string data)
        {
            throw new NotImplementedException();
        }

        public string Serialize<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public MultipartFormDataContent SerializeToMultipartFormDataContent<T>(T obj)
        {
            var boundary = $"PlutoBoundary{RandomString(16)}";
            var content = new MultipartFormDataContent(boundary);

            foreach (var property in obj.GetType().GetProperties())
            {
                if (property.GetValue(obj, null) == null) continue;

                var jsonProp = property.GetCustomAttribute<JsonPropertyAttribute>();
                var name = (jsonProp != null) ? jsonProp.PropertyName : property.Name;

                var value = property.GetValue(obj, null).ToString();
                content.Add(new StringContent(value, Encoding.UTF8), name);

            }

            return content;
        }

        /// <summary>
        /// 產生一組亂數字串
        /// </summary>
        /// <param name="length">總字元數</param>
        /// <param name="chars">來源字源(AlphanumericChars | NonAlphanumericChars)</param>
        /// <returns></returns>
        private static string RandomString(int length)
        {
            Random rand = new Random(Environment.TickCount);

            var source = Enumerable.Repeat(AlphanumericChars, length).Select(s => s[rand.Next(s.Length)]);

            return new string(source.ToArray());
        }
    }
}