using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace UtilKits.Cache
{
    internal static class MemoryCacheSource
    {
        /// <summary>
        /// 統一 Cache 來源
        /// </summary>
        internal static Lazy<IMemoryCache> Cache = new Lazy<IMemoryCache>(() => new MemoryCache(new MemoryCacheOptions()));
    }

    public class MemoryStorage<T> : ICacheStorage<T>
    {
        protected IMemoryCache Cache => MemoryCacheSource.Cache.Value;

        public MemoryStorage()
        {
        }

        /// <summary>
        /// 清除Cache內的資料
        /// </summary>
        /// <param name="key">Cache Key</param>
        public virtual void Clear(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// 刪除Cache內的資料
        /// </summary>
        /// <param name="key">Cache Key</param>
        public virtual void Delete(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// 取得 Cache Object
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns>
        /// Cache Object
        /// </returns>
        public virtual T Get(string key)
        {
            return Cache.Get<T>(key);
        }

        /// <summary>
        /// 是否有資料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool HasData(string key)
        {
            return Cache.Get(key) != null;
        }

        /// <summary>
        /// 是否為連線狀態
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public bool IsDisconnect(string key)
        {
            return true;
        }

        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="keys">鍵值清單</param>
        public void MDelete(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Delete(key);
            }
        }

        /// <summary>
        /// 取得多個鍵值下的 Cache Object
        /// </summary>
        /// <param name="keys">鍵值清單</param>
        /// <returns>
        /// Cache Object List
        /// </returns>
        public List<T> MGet(IEnumerable<string> keys)
        {
            List<T> result = new List<T>();

            foreach (var key in keys)
            {
                if (!HasData(key)) continue;

                result.Add(Get(key));
            }

            return result;
        }

        /// <summary>
        /// 設定多筆Object to Redis Cache
        /// </summary>
        /// <param name="values">KeyValue</param>
        public void MSet(IDictionary<string, T> values)
        {
            foreach (var item in values)
            {
                Set(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 設定Object to Cache
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="cacheObject">Cache Object</param>
        public void Set(string key, T cacheObject)
        {
            Cache.Set(key, cacheObject);
        }

        /// <summary>
        /// 設定Object to Cache
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="cacheObject">Cache Object</param>
        /// <param name="expiredTime">過期時間</param>
        public void Set(string key, T cacheObject, TimeSpan expiredTime)
        {
            Cache.Set(key, cacheObject, expiredTime);
        }

        /// <summary>
        /// 設定Object to Cache
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="cacheObject">Cache Object</param>
        /// <param name="options">額外設定條件</param>
        public void Set(string key, T cacheObject, MemoryCacheEntryOptions options)
        {
            Cache.Set(key, cacheObject, options);
        }

        /// <summary>
        /// 更新Cache資料
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="oldCacheObject">更新前的Cache資料</param>
        /// <param name="newCacheObject">更新後的Cache資料</param>
        /// <returns>
        /// 是否更新成功(如更新期間Cache資料已被異動則會更新失敗)
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool TryUpdate(string key, T oldCacheObject, T newCacheObject)
        {
            if (oldCacheObject.Equals(Get(key)))
            {
                Set(key, newCacheObject);
                return true;
            }

            return false;
        }
    }
}
