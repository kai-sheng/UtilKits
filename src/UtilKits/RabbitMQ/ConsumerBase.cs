using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace UtilKits.RabbitMQ
{
    public abstract class ConsumerBase<T> : RabbitMQClient
    {
        /// <summary>
        /// 訂閱 QUEUE，有更新時執行工作
        /// </summary>
        public virtual void SubscribeQueue()
        {
            base.Connect();

            try
            {
                Channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                var consumer = new AsyncEventingBasicConsumer(Channel);
                consumer.Received += OnEventReceived;
                Channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            }
            catch (Exception)
            {
                throw new Exception("Error while consuming message");
            }
        }

        /// <summary>
        /// 結束訂閱 QUEUE
        /// </summary>
        public virtual void EndSubcribe()
        {
            base.Dispose();
        }

        /// <summary>
        /// 主動讀取 QUEUE，取資料時執行工作
        /// </summary>
        public virtual async Task PullQueue()
        {
            base.Connect();

            string data = "";
            Channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            BasicGetResult result = Channel.BasicGet(queue: QueueName, autoAck: false);

            try
            {

                if (result != null)
                {
                    var body = Encoding.UTF8.GetString(result.Body.ToArray());
                    data = body;
                    var message = JsonConvert.DeserializeObject<T>(body);

                    await Invoke(message);

                    Channel.BasicAck(result.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                //把訊息退回到queue中
                Channel.BasicReject(deliveryTag: result.DeliveryTag, requeue: false);
                await ExceptionHandler(ex, data);

                throw new Exception("Error while retrieving message from queue.");
            }
            finally
            {
                base.Dispose();
            }

        }


        /// <summary>
        /// 訂閱動作(內容)
        /// </summary>
        protected virtual async Task OnEventReceived(object sender, BasicDeliverEventArgs @event)
        {
            string data = "";

            try
            {
                var body = Encoding.UTF8.GetString(@event.Body.ToArray());
                data = body;
                var message = JsonConvert.DeserializeObject<T>(body);
                await Invoke(message);
                Channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                //把訊息退回到queue中
                Channel.BasicReject(deliveryTag: @event.DeliveryTag, requeue: false);

                // var error = new ErrorMessage
                // {
                //     OriginalData = data,
                //     Exception = ex,
                //     ExchangeName = this.ExchangeName,
                //     RoutingKeyName = this.RoutingKeyName,
                //     QueueName = this.QueueName
                // };
                // new ErrorProducer(error);

                await ExceptionHandler(ex, data);
            }
        }

        /// <summary>
        /// 接收後執行動作
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        protected abstract Task Invoke(T message);

        /// <summary>
        /// 錯誤發生時執行動作
        /// </summary>
        /// <param name="ex">錯誤內容</param>
        protected abstract Task ExceptionHandler(Exception ex, string data);
    }
}
