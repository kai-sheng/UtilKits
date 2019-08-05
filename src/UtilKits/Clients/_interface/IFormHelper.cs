using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace UtilKits.Clients
{
    public interface IFormHelper
    {
        /// <summary>
        /// 使用 application/x-www-form-urlencoded
        /// 非同步傳送表單內容
        /// </summary>
        /// <typeparam name="TResult">回傳的泛型結果</typeparam>
        /// <param name="url">指定網址位置</param>
        /// <param name="postData">傳送的資料KEY-VALUE</param>
        /// <returns></returns>
        Task<HttpResponseMessage> PostAsyncUrlEncoded(string url, IEnumerable<KeyValuePair<string, string>> postData);

        /// <summary>
        /// 使用 application/x-www-form-urlencoded
        /// 傳送表單內容
        /// </summary>
        /// <typeparam name="TResult">回傳的泛型結果</typeparam>
        /// <param name="url">指定網址位置</param>
        /// <param name="postData">傳送的資料KEY-VALUE</param>
        /// <returns></returns>
        Task<TResult> PostUrlEncoded<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData);

        /// <summary>
        /// 使用 application/x-www-form-urlencoded
        /// 傳送表單內容
        /// </summary>
        /// <typeparam name="TResult">回傳的泛型結果</typeparam>
        /// <param name="url">指定網址位置</param>
        /// <param name="postData">傳送的資料KEY-VALUE</param>
        /// <returns></returns>
        Task<string> PostUrlEncoded(string url, IEnumerable<KeyValuePair<string, string>> postData);
    }
}
