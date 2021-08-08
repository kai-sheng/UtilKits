using System;
using System.Collections.Generic;
using System.Text;

namespace UtilKits.RabbitMQ
{
    public interface IProducerBase<in T>
    {
        /// <summary>
        /// 上傳 QUEUE 
        /// </summary>
        /// <param name="Source">QUEUE 字串</param>
        void Publish(T Source);
    }
}
