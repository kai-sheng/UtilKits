using System.Collections.Generic;

namespace UtilKits.Database._interface
{
    public interface IApiHelper
    {
         IEnumerable<TResponse> QueryCollection<TRequest, TResponse>(Dictionary<string, string> headerDictionary,
            string Router, TRequest data) where TResponse : class where TRequest : class;

    }
}
