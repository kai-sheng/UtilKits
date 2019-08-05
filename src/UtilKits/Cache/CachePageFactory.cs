﻿using System;

namespace UtilKits.Cache
{
    public class CachePageFactory<T> : CacheSectionFactory<T>
    {
        protected ICacheStorage<int> _totalCacheStorage;

        public CachePageFactory(string key, int pageIndex, int pageSize) : base($"{key}_S{pageSize}_P{pageIndex}", $"{key}_S{pageSize}")
        {
            _totalCacheStorage = new MemoryStorage<int>();
        }

        public CachePageFactory(string key, int pageIndex, int pageSize, TimeSpan expiration) : this(key, pageIndex, pageSize)
        {
            _expiration = expiration;
        }

        private string PageTotalKey => $"{SectionKey}_Total";

        public T Get(Func<(int Total, T Data)> bindPagingCallback)
        {
            if (!HasData())
            {
                var result = bindPagingCallback();

                _totalCacheStorage.Set(PageTotalKey, result.Total);
                Append(PageTotalKey);

                Set(result.Data);

            }

            var model = _cacheStorage.Get(StorageKey);

            return (model == null) ? default(T) : model.Data;
        }
    }
}
