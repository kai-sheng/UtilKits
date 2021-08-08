using System.Threading.Tasks;

namespace UtilKits.RabbitMQ
{
    public interface IConsumerBase<in T>
    {

        /// <summary>
        /// 主動讀取 QUEUE，取資料時執行工作
        /// </summary>
        Task PullQueue();
    }
}
