using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable
    {     
        private readonly IConnectionFactory connectionFactory;
        private readonly int retryCount;
        private IConnection connection;
        private object lock_object = new object();
        private bool _disposed;

        public bool IsConnected => connection != null && connection.IsOpen;

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            this.connectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }

        public IChannel CreateChannel()
        {
            return connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _disposed = true;
            connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                     .Or<BrokerUnreachableException>()
                     .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                     {
                        
                     });

                policy.Execute(() =>
                {
                    connection = connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
                });

                if (IsConnected)
                {
                    connection.ConnectionShutdownAsync += Connection_ConnectionShutdownAsync;
                    connection.CallbackExceptionAsync += Connection_CallbackExceptionAsync;
                    connection.ConnectionBlockedAsync += Connection_ConnectionBlockedAsync;
                    
                    return true;
                }

                return false;
            }
        }

        private async Task Connection_ConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs @event)
        {
            if (_disposed) return;
            TryConnect();
        }

        private async Task Connection_CallbackExceptionAsync(object sender, CallbackExceptionEventArgs @event)
        {         
            if (_disposed) return;
            TryConnect();
        }

        private async Task Connection_ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
        {
            if (_disposed) return;
            TryConnect();
        }
    }
}
