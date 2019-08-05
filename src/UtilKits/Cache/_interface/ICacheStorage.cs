using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace UtilKits.Cache
{
    /// <summary>
    /// 儲存庫介面
    /// MemoryStorage : 一般網站暫存
    /// RedisStorage : Redis 暫存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICacheStorage<T>
    {

        /// <summary>
        /// 是否為連線狀態
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        bool IsDisconnect(string key);

        /// <summary>
        /// 是否有資料
        /// </summary>
        /// <param name="key">鍵值</param>
        bool HasData(string key);

        /// <summary>
        /// 取得 Cache Object
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns>Cache Object</returns>
        T Get(string key);

        /// <summary>
        /// 取得多個鍵值下的 Cache Object
        /// </summary>
        /// <param name="keys">鍵值清單</param>
        /// <returns>Cache Object List</returns>
        List<T> MGet(IEnumerable<string> keys);

        /// <summary>
        /// 設定Object to Cache
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="cacheObject">Cache Object</param>
        void Set(string key, T cacheObject);

        /// <summary>
        /// 設定Object to Cache
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="cacheObject">Cache Object</param>
        /// <param name="expiredTime">過期時間</param>
        void Set(string key, T cacheObject, TimeSpan expiredTime);

        /// <summary>
        /// 設定Object to Cache
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="cacheObject">Cache Object</param>
        /// <param name="options">額外設定條件</param>
        void Set(string key, T cacheObject, MemoryCacheEntryOptions options);

        /// <summary>
        /// 設定多筆Object to Redis Cache
        /// </summary>
        /// <param name="values">KeyValue</param>
        void MSet(IDictionary<string, T> values);

        /// <summary>
        /// 更新Cache資料
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="oldCacheObject">更新前的Cache資料</param>
        /// <param name="newCacheObject">更新後的Cache資料</param>
        /// <returns>是否更新成功(如更新期間Cache資料已被異動則會更新失敗)</returns>
        bool TryUpdate(string key, T oldCacheObject, T newCacheObject);

        /// <summary>
        /// 刪除Cache內的資料
        /// </summary>
        /// <param name="key">Cache Key</param>
        void Delete(string key);

        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="keys">鍵值清單</param>
        void MDelete(IEnumerable<string> keys);
    }
}
