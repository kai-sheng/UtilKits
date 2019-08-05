using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace UtilKits.Clients
{
    public class FormHelper : IFormHelper
    {
        private static Lazy<IFormHelper> _helper = new Lazy<IFormHelper>(() => new FormHelper());

        public static IFormHelper Instance
        {
            get { return _helper.Value; }
            internal set { _helper = new Lazy<IFormHelper>(() => value); }
        }

        /// <summary>
        /// 使用 application/x-www-form-urlencoded
        /// 非同步傳送表單內容
        /// </summary>
        /// <typeparam name="TResult">回傳的泛型結果</typeparam>
        /// <param name="url">指定網址位置</param>
        /// <param name="postData">傳送的資料KEY-VALUE</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsyncUrlEncoded(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            using (var httpClient = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(postData))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    return await httpClient.PostAsync(url, content).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// 使用 application/x-www-form-urlencoded
        /// 傳送表單內容
        /// </summary>
        /// <typeparam name="TResult">回傳的泛型結果</typeparam>
        /// <param name="url">指定網址位置</param>
        /// <param name="postData">傳送的資料KEY-VALUE</param>
        /// <returns></returns>
        public async Task<TResult> PostUrlEncoded<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var response = await PostAsyncUrlEncoded(url, postData);

            return await response.Content.ReadAsAsync<TResult>();
        }

        /// <summary>
        /// 使用 application/x-www-form-urlencoded
        /// 傳送表單內容
        /// </summary>
        /// <typeparam name="TResult">回傳的泛型結果</typeparam>
        /// <param name="url">指定網址位置</param>
        /// <param name="postData">傳送的資料KEY-VALUE</param>
        /// <returns></returns>
        public async Task<string> PostUrlEncoded(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var response = await PostAsyncUrlEncoded(url, postData);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
