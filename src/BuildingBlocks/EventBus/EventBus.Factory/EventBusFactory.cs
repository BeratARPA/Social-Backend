using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;

namespace EventBus.Factory
{
    public class EventBusFactory
    {
        public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {
            return config.EventBusType switch
            {
                EventBusType.RabbitMQ => new EventBusRabbitMQ(config, serviceProvider),
                EventBusType.AzureServiceBus => throw new NotSupportedException($"Event bus type '{config.EventBusType}' is not supported."), //new EventBusAzureServiceBus(config, serviceProvider),
                EventBusType.Kafka => throw new NotSupportedException($"Event bus type '{config.EventBusType}' is not supported."), // new EventBusKafka(config, serviceProvider),
                _ => throw new NotSupportedException($"Event bus type '{config.EventBusType}' is not supported.")
            };
        }
    }
}
