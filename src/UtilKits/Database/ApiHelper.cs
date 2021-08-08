using System;
using System.Collections.Generic;
using UtilKits.Clients;
using UtilKits.Database._interface;

namespace UtilKits.Database
{
    public class ApiHelper : ApiClient, IApiHelper
    {
        public IEnumerable<TResponse> QueryCollection<TRequest, TResponse>(Dictionary<string, string> headerDictionary,
            string Router, TRequest data) where TResponse : class where TRequest : class
        {
            HttpHeader = headerDictionary;
            return this.Post<TRequest, TResponse>(new Uri(Router), data) as List<TResponse>;
        }
    }
}
