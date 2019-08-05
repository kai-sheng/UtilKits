namespace UtilKits.Cache
{
    public static class CacheFactoryExtension
    {
        /// <summary>
        /// 是否超過暫存時間
        /// 當主要時間 > 關聯時間 即為超過，反之則無
        /// </summary>
        /// <param name="source">主要暫存物件</param>
        /// <param name="target">關聯暫存物件</param>
        /// <typeparam name="T">主要暫存泛型</typeparam>
        /// <typeparam name="U">關聯暫存泛型</typeparam>
        /// <returns></returns>
        public static bool OverDependencyTime<T, U>(this CacheFactory<T> source, CacheFactory<U> target)
        {
            var targetTime = target.GetCacheTime();
            var sourceTime = source.GetCacheTime();

            return targetTime.HasValue && sourceTime.HasValue && targetTime > sourceTime;
        }
    }
}