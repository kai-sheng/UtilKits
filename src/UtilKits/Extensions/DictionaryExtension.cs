using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilKits.Extensions
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// 依Value 取得 Keys
        /// </summary>
        /// <param name="source">來源</param>
        /// <param name="value">數值</param>
        /// <typeparam name="TKey">鍵值泛型</typeparam>
        /// <typeparam name="TValue">數值泛型</typeparam>
        /// <returns></returns>
        public static IEnumerable<TKey> GetKeys<TKey, TValue>(this IDictionary<TKey, TValue> source, TValue value)
        {
            if (source == null)
                throw new ArgumentNullException("dictionary");

            foreach (var pair in source.Where(s => s.Value.Equals(value)))
            {
                yield return pair.Key;
            }
        }

        /// <summary>
        /// 依 Key 取得 Value
        /// </summary>
        /// <param name="source">來源</param>
        /// <param name="key">鍵值</param>
        /// <typeparam name="TKey">鍵值泛型</typeparam>
        /// <typeparam name="TValue">數值泛型</typeparam>
        /// <returns></returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return source.ContainsKey(key) ? source[key] : default(TValue);
        }
    }
}
