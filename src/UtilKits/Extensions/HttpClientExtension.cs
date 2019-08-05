using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace UtilKits.Extensions
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// 設定 MediaTypeHeaderValue
        /// </summary>
        /// <param name="client">The HttpClient</param>
        internal static HttpClient SetMediaTypeHeaderValue(this HttpClient client, MediaTypeWithQualityHeaderValue headerValue)
        {
            if (headerValue != null)
            {
                client.DefaultRequestHeaders.Accept.Add(headerValue);
            }

            return client;
        }

        /// <summary>
        /// 設定 Http header
        /// </summary>
        /// <param name="client">The HttpClient</param>
        internal static HttpClient SetHttpHeader(this HttpClient client, Dictionary<string, string> httpHeader)
        {
            if (httpHeader != null && httpHeader.Any())
            {
                foreach (var item in httpHeader)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            return client;
        }
    }
}
