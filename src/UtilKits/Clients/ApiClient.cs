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
        public ApiClient(
            SerializerRequestFormat requestFormat = SerializerRequestFormat.Json, 
            SerializerResponseFormat responseFormat = SerializerResponseFormat.Json)
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
        public enum SerializerRequestFormat
        {
            Json,
            Xml,
            Ini,
            FormUrlencode,
            Multipart
        }

        /// <summary>
        /// 序列化格式列舉
        /// </summary>
        public enum SerializerResponseFormat
        {
            Json,
            Xml,
            Ini,
            None
        }

        /// <summary>
        /// The Http ResponseBody
        /// </summary>
        public string ResponseBody { get; private set; }

        /// <summary>
        /// The Http Status Code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

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
        protected SerializerRequestFormat _requestFormat;
        /// <summary>
        /// 格式化類型(Response)
        /// </summary>
        protected SerializerResponseFormat _responseFormat;

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
        public TResponse Get<TResponse>(Uri uri) where TResponse : class
        {
            return Send<TResponse>((client) => client.GetAsync(uri).Result);
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI (不含內容)
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Uri</param>
        /// <returns>The Response Entity</returns>
        public TResponse Post<TResponse>(Uri uri) where TResponse : class
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
        public TResponse Post<TResponse>(Uri uri, string content) where TResponse : class
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
        public TResponse Post<TRequest, TResponse>(Uri uri, TRequest request) where TResponse : class where TRequest : class
        {
            var httpContent = GetRequestContent(request);
            
            return Send<TResponse>((client) => client.PostAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI
        /// 不反序列化回傳內容
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="request">POST的object</param>
        /// <returns></returns>
        public bool Post<TRequest>(Uri uri, TRequest request) where TRequest : class
        {
            var httpContent = GetRequestContent(request);

            return Send((client) => client.PostAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Ur</param>
        /// <param name="httpContent">The HttpContent</param>
        /// <returns>The Response Entity</returns>
        public TResponse Post<TResponse>(Uri uri, HttpContent httpContent) where TResponse : class
        {
            return Send<TResponse>((client) => client.PostAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 PUT 要求傳送至指定的 URI
        /// </summary>
        /// <param name="uri">The RequestUri</param>
        /// <param name="httpContent">The HttpContent</param>
        public TResponse Put<TResponse>(Uri uri, HttpContent httpContent) where TResponse : class
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
        public TResponse Put<TRequest, TResponse>(Uri uri, TRequest request) where TResponse : class where TRequest : class
        {
            var httpContent = GetRequestContent(request);
            
            return Send<TResponse>((client) => client.PutAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 PUT 要求傳送至指定的 URI
        /// 不反序列化回傳內容
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="request">PUT的object</param>
        /// <returns></returns>
        public bool Put<TRequest>(Uri uri, TRequest request) where TRequest : class
        {
            var httpContent = GetRequestContent(request);

            return Send((client) => client.PutAsync(uri, httpContent).Result);
        }

        /// <summary>
        /// 將 DELETE 要求傳送至指定的 URI
        /// </summary>
        /// <param name="uri">The RequestUri</param>
        public TResponse Delete<TResponse>(Uri uri, HttpContent httpContent) where TResponse : class
        {
            return Send<TResponse>((client) => client.DeleteAsync(uri).Result);
        }

        /// <summary>
        /// 將 DELETE 要求傳送至指定的 URI
        /// 不反序列化回傳內容
        /// </summary>
        /// <param name="uri">The RequestUri</param>
        public bool Delete(Uri uri, HttpContent httpContent)
        {
            return Send((client) => client.DeleteAsync(uri).Result);
        }

        /// <summary>
        /// 依Request Format 轉換為 HttpContent
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private HttpContent GetRequestContent<TRequest>(TRequest request) where TRequest : class
        {
            switch (_requestFormat)
            {
                case SerializerRequestFormat.FormUrlencode:
                    return (_requestSerializer as FormUrlencodeHelper).SerializeToFormUrlEncodedContent(request);
                case SerializerRequestFormat.Multipart:
                    return (_requestSerializer as MultipartFormHelper).SerializeToMultipartFormDataContent(request);
                default:
                    return new StringContent(((object)request).ToJson(), Encoding.UTF8, MediaTypeHeaderValue.MediaType);
            }
        }

        /// <summary>
        /// 將 HttpContent 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="action">執行的動作</param>
        /// <returns>The Response Entity</returns>
        protected virtual TResponse Send<TResponse>(Func<HttpClient, HttpResponseMessage> action) where TResponse : class
        {
            if (Send(action) && _responseDeserializer != null)
            {
                return _responseDeserializer.Deserialize<TResponse>(this.ResponseBody);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 將 HttpContent 要求傳送至指定的 URI
        /// 不反序列化回傳內容
        /// </summary>
        /// <param name="action">執行的動作</param>
        /// <returns>回傳是否成功</returns>
        protected virtual bool Send(Func<HttpClient, HttpResponseMessage> action)
        {
            using (var client = new HttpClient())
            {
                client.MaxResponseContentBufferSize = MaxResponseContentBufferSize;
                client.Timeout = Timeout;
                client.SetHttpHeader(HttpHeader)
                    .SetMediaTypeHeaderValue(MediaTypeHeaderValue);
                
                var response = action(client);

                ResponseBody = response.Content.ReadAsStringAsync().Result;
                StatusCode = response.StatusCode;

                return response.IsSuccessStatusCode;
            }
        }

        /// <summary>
        /// 取得實作序列化的物件
        /// </summary>
        /// <param name="format">序列化的格式</param>
        /// <returns>實作序列化物件</returns>
        private ISerializer GetSerialzar(SerializerRequestFormat format)
        {
            switch (format)
            {
                case SerializerRequestFormat.Json:
                    return JsonHelper.Instance;
                case SerializerRequestFormat.Xml:
                    return XmlHelper.Instance;
                case SerializerRequestFormat.Ini:
                    return INIHelper.Instance;
                case SerializerRequestFormat.FormUrlencode:
                    return FormUrlencodeHelper.Instance;
                case SerializerRequestFormat.Multipart:
                    return MultipartFormHelper.Instance;
                default:
                    throw new NotImplementedException("Request Serializer Format Not Implemented.");
            }
        }

        /// <summary>
        /// 取得實作序列化的物件
        /// </summary>
        /// <param name="format">序列化的格式</param>
        /// <returns>實作序列化物件</returns>
        private ISerializer GetSerialzar(SerializerResponseFormat format)
        {
            switch (format)
            {
                case SerializerResponseFormat.Json:
                    return JsonHelper.Instance;
                case SerializerResponseFormat.Xml:
                    return XmlHelper.Instance;
                case SerializerResponseFormat.Ini:
                    return INIHelper.Instance;
                case SerializerResponseFormat.None:
                    return null;
                default:
                    throw new NotImplementedException("Response Serializer Format Not Implemented.");
            }
        }

        /// <summary>
        /// 初始化MediaTypeWithQualityHeaderValue
        /// </summary>
        /// <param name="format">序列化的格式</param>
        private MediaTypeWithQualityHeaderValue GetMediaTypeHeaderValue(SerializerRequestFormat format)
        {
            switch (format)
            {
                case SerializerRequestFormat.Json:
                    return new MediaTypeWithQualityHeaderValue("application/json");
                case SerializerRequestFormat.Xml:
                    return new MediaTypeWithQualityHeaderValue("application/xml");
                case SerializerRequestFormat.Ini:
                    return new MediaTypeWithQualityHeaderValue("text/plain");
                case SerializerRequestFormat.FormUrlencode:
                    return new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
                case SerializerRequestFormat.Multipart:
                    return new MediaTypeWithQualityHeaderValue("multipart/form-data");
                default:
                    throw new NotImplementedException("Serializer Format Not Implemented.");
            }
        }
    }
    
}
