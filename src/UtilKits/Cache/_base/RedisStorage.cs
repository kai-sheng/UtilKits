using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;

namespace UtilKits.Cache
{
    public class RedisStorage<T> : ICacheStorage<T>
    {
        protected IDistributedCache Cache;
        public RedisStorage(string host)
        {
            Cache = new RedisCache(new RedisCacheOptions { Configuration = host });
        }

        public void Delete(string key)
        {

            Cache.Remove(key);
        }

        public T Get(string key)
        {
            return ByteArrayToObject<T>(Cache.Get(key));
        }

        public bool HasData(string key)
        {
            return Cache.Get(key) != null;
        }

        public bool IsDisconnect(string key)
        {
            return true;
        }

        public void MDelete(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Delete(key);
            }
        }

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

        public void MSet(IDictionary<string, T> values)
        {
            foreach (var item in values)
            {
                Set(item.Key, item.Value);
            }
        }

        public void Set(string key, T cacheObject)
        {
            Cache.Set(key, ObjectToByteArray(cacheObject));
        }

        public void Set(string key, T cacheObject, TimeSpan expiredTime)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = expiredTime;

            Cache.Set(key, ObjectToByteArray(cacheObject), options);
        }

        public void Set(string key, T cacheObject, MemoryCacheEntryOptions options)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            };
            Cache.Set(key, ObjectToByteArray(cacheObject), cacheOptions);
        }

        public bool TryUpdate(string key, T oldCacheObject, T newCacheObject)
        {
            if (oldCacheObject.Equals(Get(key)))
            {
                Set(key, newCacheObject);
                return true;
            }
            return false;
        }
        private byte[] ObjectToByteArray(object obj)
        {

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                var jsonObject = JsonConvert.SerializeObject(obj);
                binaryFormatter.Serialize(memoryStream, jsonObject);
                return memoryStream.ToArray();
            }
        }

        private T ByteArrayToObject<T>(byte[] bytes)
        {

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                memoryStream.Write(bytes, 0, bytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var obj = binaryFormatter.Deserialize(memoryStream);
                return JsonConvert.DeserializeObject<T>(obj.ToString());
            }
        }
    }
}
