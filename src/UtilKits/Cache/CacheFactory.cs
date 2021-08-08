using System;
using Microsoft.Extensions.Caching.Memory;

namespace UtilKits.Cache
{
    public abstract class CacheFactory<T>
    {
        public abstract string host { get; }
        public abstract bool isUseMemoryStorage { get; }
        protected string _key;
        protected ICacheStorage<CacheModel<T>> _cacheStorage;
        protected TimeSpan? _expiration;

        protected CacheFactory(string key)
        {
            _key = key;
            if (isUseMemoryStorage)
            {
                _cacheStorage = new MemoryStorage<CacheModel<T>>();
            }
            else
            {
                _cacheStorage = new RedisStorage<CacheModel<T>>(host);
            }
            // 若使用 reids 在此判斷連線狀態，轉換為MemoryCache
        }

        protected CacheFactory(string key, TimeSpan expiration) : this(key)
        {
            _expiration = expiration;
        }

        protected virtual string StorageKey => _key;

        public virtual void Bind()
        { }

        /// <summary>
        /// 刪除Cache內的資料
        /// </summary>
        public void Delete()
        {
            _cacheStorage.Delete(StorageKey);

            RemoveDependancyCache();
        }

        /// <summary>
        /// 刪除Cache內的資料
        /// </summary>
        public void DeleteWithoutDependancy()
        {
            _cacheStorage.Delete(StorageKey);
        }

        /// <summary>
        /// 刪除與此Cache有關聯的所有Cache
        /// </summary>
        protected virtual void RemoveDependancyCache()
        { }

        /// <summary>
        /// 取得 Cache Object
        /// </summary>
        /// <returns>
        /// Cache Object
        /// </returns>
        public T Get()
        {
            if (!HasData())
                Bind();

            var model = _cacheStorage.Get(StorageKey);

            return (model == null) ? default(T) : model.Data;
        }

        /// <summary>
        /// 取得 Cache Object
        /// </summary>
        /// <param name="bindCallback">若無資料的建置內容</param>
        /// <returns>
        /// Cache Object
        /// </returns>
        public T Get(Func<T> bindCallback)
        {
            if (!HasData())
                Set(bindCallback());

            var model = _cacheStorage.Get(StorageKey);

            return (model == null) ? default(T) : model.Data;
        }

        /// <summary>
        /// 取得 Cache Object
        /// </summary>
        /// <param name="bindCallback">若無資料的建置內容</param>
        /// <param name="isRefreshCallback">是否需要更新判斷</param>
        /// <returns>
        /// Cache Object
        /// </returns>
        public T Get(Func<T> bindCallback, Func<bool> isRefreshCallback)
        {
            if (!HasData() || isRefreshCallback())
                Set(bindCallback());

            var model = _cacheStorage.Get(StorageKey);

            return (model == null) ? default(T) : model.Data;
        }

        /// <summary>
        /// 取得 Cache Object 的時間
        /// </summary>
        /// <returns>
        /// 無暫存時，回傳NULL
        /// 有暫存但資料為空時，回傳最小時間
        /// 有暫存也有資料時，回傳暫存時間
        /// </returns>
        public DateTime? GetCacheTime()
        {
            if (!HasData()) return null;

            dynamic model = _cacheStorage.Get(StorageKey);

            return (model == null) ? DateTime.MinValue : model.CacheTime;
        }

        /// <summary>
        /// 是否有資料
        /// </summary>
        /// <returns></returns>
        public bool HasData()
        {
            return _cacheStorage.HasData(StorageKey);
        }

        /// <summary>
        /// 設定Object to Cache
        /// </summary>
        /// <param name="cacheObject">Cache Object</param>
        public virtual void Set(T cacheObject)
        {
            if (cacheObject == null) return;

            var options = new MemoryCacheEntryOptions();

            if (_expiration.HasValue)
                options.SetSlidingExpiration(_expiration.Value);

            options.RegisterPostEvictionCallback((echoKey, value, reason, substate) => {
                RemoveDependancyCache();
            });

            _cacheStorage.Set(StorageKey, new CacheModel<T> { Data = cacheObject, CacheTime = DateTime.UtcNow }, options);
        }
    }
}
