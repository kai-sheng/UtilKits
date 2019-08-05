using System;

namespace UtilKits.Cache
{
    public class CacheModel<T>
    {
        /// <summary>
        /// 暫存資料內容
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 暫存時間
        /// </summary>
        public DateTime CacheTime { get; set; }
    }
}