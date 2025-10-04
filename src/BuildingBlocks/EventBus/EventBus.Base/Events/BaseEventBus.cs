using EventBus.Base.Abstraction;
using EventBus.Base.SubscriptionManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus, IDisposable
    {
        public readonly IServiceProvider ServiceProvider;
        public readonly IEventBusSubscriptionManager SubscriptionManager;

        public EventBusConfig EventBusConfig { get; private set; }

        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            EventBusConfig = config;
            ServiceProvider = serviceProvider;
            SubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        }

        public virtual string ProcessEventName(string eventName)
        {
            if (EventBusConfig.DeleteEventPrefix && eventName.StartsWith(EventBusConfig.EventNamePrefix))
                eventName = eventName.Substring(EventBusConfig.EventNamePrefix.Length);

            if (EventBusConfig.DeleteEventSuffix && eventName.EndsWith(EventBusConfig.EventNameSuffix))
                eventName = eventName.Substring(0, eventName.Length - EventBusConfig.EventNameSuffix.Length);

            return eventName;
        }

        public virtual string GetSubName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }

        public virtual void Dispose()
        {
            EventBusConfig = null;
            SubscriptionManager.Clear();
        }

        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            eventName = ProcessEventName(eventName);
            Console.WriteLine($"[DEBUG] Processed Event Name: {eventName}");

            var processed = false;

            if (SubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                Console.WriteLine($"[DEBUG] Subscription exists for event '{eventName}': TRUE");
                var subscriptions = SubscriptionManager.GetHandlersForEvent(eventName);

                using (var scope = ServiceProvider.CreateScope())
                {
                    foreach (var subscription in subscriptions)
                    {
                        Console.WriteLine($"[DEBUG] Found subscription: {subscription.HandlerType.FullName}");
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        if (handler == null)
                        {
                            Console.WriteLine($"[ERROR] Handler could NOT be resolved: {subscription.HandlerType.FullName}");
                            continue;
                        }

                        Console.WriteLine($"[DEBUG] Handler resolved: {subscription.HandlerType.FullName}");

                        var eventType = SubscriptionManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");
                        Console.WriteLine($"[DEBUG] Event type to deserialize: {eventType.FullName}");

                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        Console.WriteLine($"[DEBUG] Event deserialized");

                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        var method = concreteType.GetMethod("Handle");

                        if (method == null)
                        {
                            Console.WriteLine($"[ERROR] 'Handle' method not found on {concreteType.FullName}");
                            continue;
                        }

                        Console.WriteLine($"[DEBUG] Invoking Handle method...");
                        var task = (Task)method?.Invoke(handler, new object[] { integrationEvent });
                        if (task != null)
                        {
                            await task;
                            Console.WriteLine($"[DEBUG] Handler executed successfully.");
                        }
                    }
                }

                processed = true;
            }
            else
            {
                Console.WriteLine($"[WARN] No subscription found for event: {eventName}");
            }

            return processed;
        }

        public abstract void Publish(IntegrationEvent @event);

        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        public abstract void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}