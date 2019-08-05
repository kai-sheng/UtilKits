using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UtilKits.Extensions;
using UtilKits.Serializer;

namespace UtilKits.Clients
{
    public abstract class ApiClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientServiceBase" /> class.
        /// </summary>
        /// <param name="requestFormat">序列化格式</param>
        public ApiClient(SerializerFormat requestFormat = SerializerFormat.Json, SerializerFormat responseFormat = SerializerFormat.Json)
        {
            _requestFormat = requestFormat;
            _responseFormat = responseFormat;

            _requestSerializer = this.GetSerialzar(requestFormat);
            _responseDeserializer = this.GetSerialzar(responseFormat);

            MediaTypeHeaderValue = GetMediaTypeHeaderValue(requestFormat);
            MaxResponseContentBufferSize = int.MaxValue;
            Timeout = TimeSpan.FromSeconds(100);

            HttpHeader = new Dictionary<string, string>();

        }

        /// <summary>
        /// 序列化格式列舉
        /// </summary>
        public enum SerializerFormat
        {
            Json,
            Xml,
            Ini,
            FormUrlencode,
            Multipart
        }

        /// <summary>
        /// The Http ResponseBody
        /// </summary>
        public string ResponseBody { get; private set; }

        /// <summary>
        /// The Http Status Code
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; private set; }

        /// <summary>
        /// Gets or sets the maximum number of bytes to buffer when reading the response content.
        /// </summary>
        public long MaxResponseContentBufferSize { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// The Media Type Header Value
        /// </summary>
        public MediaTypeWithQualityHeaderValue MediaTypeHeaderValue { get; set; }

        /// <summary>
        /// Gets or sets the http header
        /// </summary>
        public Dictionary<string, string> HttpHeader { get; set; }

        /// <summary>
        /// 格式化類型(Request)
        /// </summary>
        protected SerializerFormat _requestFormat;
        /// <summary>
        /// 格式化類型(Response)
        /// </summary>
        private SerializerFormat _responseFormat;

        /// <summary>
        /// 實作序列化的物件(Request)
        /// </summary>
        protected ISerializer _requestSerializer;
        /// <summary>
        /// 實作序列化的物件(Response)
        /// </summary>
        private ISerializer _responseDeserializer;

        /// <summary>
        /// 將 GET 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Uri</param>
        /// <returns>The Response Entity</returns>
        protected virtual TResponse Get<TResponse>(Uri uri) where TResponse : class
        {
            return Send<TResponse>((client) => client.GetAsync(uri).Result);
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI (不含內容)
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Uri</param>
        /// <returns>The Response Entity</returns>
        protected virtual TResponse Post<TResponse>(Uri uri) where TResponse : class
        {
            var httpContent = new StringContent(string.Empty);

            return Send<TResponse>((client) => client.PostAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Uri</param>
        /// <param name="content">POST的內容</param>
        /// <returns>The Response Entity</returns>
        protected virtual TResponse Post<TResponse>(Uri uri, string content) where TResponse : class
        {
            var httpContent = new StringContent(content, Encoding.UTF8, MediaTypeHeaderValue.MediaType);

            return Send<TResponse>((client) => client.PostAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="request">POST的object</param>
        /// <returns></returns>
        protected virtual TResponse Post<TRequest, TResponse>(Uri uri, TRequest request) where TResponse : class where TRequest : class
        {
            var httpContent = GetRequestContent(request);
            
            return Send<TResponse>((client) => client.PostAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Ur</param>
        /// <param name="httpContent">The HttpContent</param>
        /// <returns>The Response Entity</returns>
        protected virtual TResponse Post<TResponse>(Uri uri, HttpContent httpContent) where TResponse : class
        {
            return Send<TResponse>((client) => client.PostAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 PUT 要求傳送至指定的 URI
        /// </summary>
        /// <param name="uri">The RequestUri</param>
        /// <param name="httpContent">The HttpContent</param>
        protected virtual TResponse Put<TResponse>(Uri uri, HttpContent httpContent) where TResponse : class
        {
            return Send<TResponse>((client) => client.PutAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 PUT 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="request">PUT的object</param>
        /// <returns></returns>
        protected virtual TResponse Put<TRequest, TResponse>(Uri uri, TRequest request) where TResponse : class where TRequest : class
        {
            var httpContent = GetRequestContent(request);
            
            return Send<TResponse>((client) => client.PutAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 DELETE 要求傳送至指定的 URI
        /// </summary>
        /// <param name="uri">The RequestUri</param>
        protected virtual TResponse Delete<TResponse>(Uri uri, HttpContent httpContent) where TResponse : class
        {
            return Send<TResponse>((client) => client.DeleteAsync(uri).Result);
        }

        /// <summary>
        /// 依Request Format 轉換為 HttpContent
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        protected HttpContent GetRequestContent<TRequest>(TRequest request) where TRequest : class
        {
            switch (_requestFormat)
            {
                case SerializerFormat.FormUrlencode:
                    return (_requestSerializer as FormUrlencodeHelper).SerializeToFormUrlEncodedContent(request);
                case SerializerFormat.Multipart:
                    return (_requestSerializer as MultipartFormHelper).SerializeToMultipartFormDataContent(request);
                default:
                    return new StringContent(((object)request).ToJson(), Encoding.UTF8, MediaTypeHeaderValue.MediaType);
            }
        }

        /// <summary>
        /// 將 HttpContent 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Ur</param>
        /// <param name="httpContent">The HttpContent</param>
        /// <returns>The Response Entity</returns>
        protected virtual TResponse Send<TResponse>(Func<HttpClient, HttpResponseMessage> action) where TResponse : class
        {
            using (var client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = MaxResponseContentBufferSize;
                client.Timeout = Timeout;
                client.SetHttpHeader(HttpHeader)
                    .SetMediaTypeHeaderValue(MediaTypeHeaderValue);

                var response = action(client);

                ResponseBody = response.Content.ReadAsStringAsync().Result;
                HttpStatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    return _responseDeserializer.Deserialize<TResponse>(this.ResponseBody);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 取得實作序列化的物件
        /// </summary>
        /// <param name="format">序列化的格式</param>
        /// <returns>實作序列化物件</returns>
        private ISerializer GetSerialzar(SerializerFormat format)
        {
            switch (format)
            {
                case SerializerFormat.Json:
                    return JsonHelper.Instance;
                case SerializerFormat.Xml:
                    return XmlHelper.Instance;
                case SerializerFormat.Ini:
                    return INIHelper.Instance;
                case SerializerFormat.FormUrlencode:
                    return FormUrlencodeHelper.Instance;
                case SerializerFormat.Multipart:
                    return MultipartFormHelper.Instance;
                default:
                    throw new NotImplementedException("Serializer Format Not Implemented.");
            }
        }

        /// <summary>
        /// 初始化MediaTypeWithQualityHeaderValue
        /// </summary>
        /// <param name="format">序列化的格式</param>
        private MediaTypeWithQualityHeaderValue GetMediaTypeHeaderValue(SerializerFormat format)
        {
            switch (format)
            {
                case SerializerFormat.Json:
                    return new MediaTypeWithQualityHeaderValue("application/json");
                case SerializerFormat.Xml:
                    return new MediaTypeWithQualityHeaderValue("application/xml");
                case SerializerFormat.Ini:
                    return new MediaTypeWithQualityHeaderValue("text/plain");
                case SerializerFormat.FormUrlencode:
                    return new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
                case SerializerFormat.Multipart:
                    return new MediaTypeWithQualityHeaderValue("multipart/form-data");
                default:
                    throw new NotImplementedException("Serializer Format Not Implemented.");
            }
        }
    }
    
}
