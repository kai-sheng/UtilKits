using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace UtilKits.RabbitMQ
{
    public abstract class RabbitMQClient : IDisposable
    {
        protected abstract string ExchangeName { get; }
        protected abstract ConnectionFactory ConnectionFactory { get; }
        protected abstract string RoutingKeyName { get; }
        protected abstract string QueueName { get; }
        protected IModel Channel { get; private set; }
        private IConnection _connection;

        public void Connect()
        {
            if (_connection == null || _connection.IsOpen == false)
            {
                _connection = ConnectionFactory.CreateConnection();
            }

            if (Channel == null || Channel.IsOpen == false)
            {
                Channel = _connection.CreateModel();
                Channel.ExchangeDeclare(
                    exchange: ExchangeName,
                    type: "direct",
                    durable: true,
                    autoDelete: false);

                Channel.QueueDeclare(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: new Dictionary<string, object>() { { "x-max-priority", 10 } });

                Channel.QueueBind(
                    queue: QueueName,
                    exchange: ExchangeName,
                    routingKey: RoutingKeyName);
            }
        }

        public void Dispose()
        {
            try
            {
                Channel?.Close();
                Channel?.Dispose();
                Channel = null;

                _connection?.Close();
                _connection?.Dispose();
                _connection = null;
            }
            catch (Exception)
            {
                throw new Exception("Cannot dispose RabbitMQ channel or connection");
            }
        }
    }
}
