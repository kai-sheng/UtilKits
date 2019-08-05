using System;
using System.Collections.Generic;

namespace UtilKits.Cache
{
    public abstract class CacheSectionFactory<T> : CacheFactory<T>
    {
        protected string _sectionKey;
        protected ICacheStorage<List<string>> _sectionCacheStorage;

        protected CacheSectionFactory(string key, string sectionKey) : base(key)
        {
            _sectionKey = sectionKey;
            _sectionCacheStorage = new MemoryStorage<List<string>>();
        }

        protected CacheSectionFactory(string key, string sectionKey, TimeSpan expiration) : this(key, sectionKey)
        {
            _expiration = expiration;
        }

        protected virtual string SectionKey => _sectionKey;

        /// <summary>
        /// 刪除Cache 相依的其他分頁
        /// </summary>
        protected override void RemoveDependancyCache()
        {
            var pageKeys = _sectionCacheStorage.Get(SectionKey);

            if (pageKeys != null)
            {
                foreach (var pageKey in pageKeys)
                {
                    if (pageKey == StorageKey) continue;

                    _cacheStorage.Delete(pageKey);
                }

                _sectionCacheStorage.Delete(SectionKey);
            }
        }

        /// <summary>
        /// 設定Object to Cache
        /// 同定同時會紀錄同鍵值的其他分頁資料
        /// </summary>
        /// <param name="cacheObject">Cache Object</param>
        public override void Set(T cacheObject)
        {
            base.Set(cacheObject);

            if (HasData())
                Append(StorageKey);
        }

        /// <summary>
        /// 新增分頁鍵值至Cache
        /// </summary>
        /// <param name="pageKey">分頁鍵值</param>
        protected void Append(string pageKey)
        {
            if (!_sectionCacheStorage.HasData(SectionKey))
                _sectionCacheStorage.Set(SectionKey, new List<string> { pageKey });
            else
            {
                var pageKeys = _sectionCacheStorage.Get(SectionKey);

                if (!pageKeys.Contains(pageKey))
                {
                    pageKeys.Add(pageKey);

                    _sectionCacheStorage.Set(SectionKey, pageKeys);
                }
            }
        }
    }
}
