using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace UtilKits.RabbitMQ
{
    public abstract class ProducerBase<T> : RabbitMQClient, IProducerBase<T>
    {
        protected abstract byte Priority { get; set; }

        public void Publish(T Source)
        {
            base.Connect();

            try
            {
                Channel.TxSelect();
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Source));
                var properties = Channel.CreateBasicProperties();
                //  properties.AppId = AppId;
                //properties.ContentType = "application/json";
                // properties.DeliveryMode = 1; // Doesn't persist to disk
                //   properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                properties.Persistent = true;
                properties.Priority = Priority;
                Channel.BasicPublish(exchange: ExchangeName, routingKey: RoutingKeyName, body: body, basicProperties: properties);
                Channel.TxCommit();
            }
            catch (Exception ex)
            {
                Channel.TxRollback();
                throw new Exception($"Error while publishing {ex}");
            }

            base.Dispose();
        }
    }
}
