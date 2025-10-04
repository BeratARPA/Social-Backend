namespace EventBus.Base
{
    public class EventBusConfig
    {
        public int ConnectionRetryCount { get; set; } = 5;
        public string DefaultTopicName { get; set; } = "SocialAppEventBus";
        public string EventBusConnectionString { get; set; } = string.Empty;
        public string SubscriberClientAppName { get; set; } = string.Empty;
        public string EventNamePrefix { get; set; } = "";
        public string EventNameSuffix { get; set; } = "IntegrationEvent";
        public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;
        public object Connection { get; set; }
        public bool DeleteEventPrefix => false;
        public bool DeleteEventSuffix => true;
    }

    public enum EventBusType
    {
        RabbitMQ = 1,
        AzureServiceBus = 2,
        Kafka = 3
    }
}
