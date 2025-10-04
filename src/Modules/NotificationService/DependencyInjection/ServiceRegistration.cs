using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.IntegrationEvents;
using NotificationService.Events.Handlers;
using NotificationService.Services;
using RabbitMQ.Client;

namespace NotificationService.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddNotificationService(this IServiceCollection services, IConfiguration baseConfiguration)
        {
            // 1. notification.settings.json yolunu belirle
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var notificationSettingsPath = Path.Combine(AppContext.BaseDirectory, $"notification.settings.{environment}.json");
            if (!File.Exists(notificationSettingsPath))
                throw new FileNotFoundException("notification.settings.json bulunamadı", notificationSettingsPath);

            // 2. notification.settings.json'u yükle
            var notificationConfiguration = new ConfigurationBuilder()
                .AddJsonFile(notificationSettingsPath, optional: false, reloadOnChange: true)
                .Build();

            // 3. gerekiyorsa global config ile birleştir (opsiyonel)
            var mergedConfiguration = new ConfigurationBuilder()
                .AddConfiguration(baseConfiguration)
                .AddConfiguration(notificationConfiguration)
                .Build();

            services.AddSingleton<IConfiguration>(mergedConfiguration);

            // Handler'ları Scoped olarak kaydet
            services.AddScoped<SendVerificationCodeEventHandler>();

            // Service'leri kaydet
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<IWhatsAppSender, WhatsAppSender>();
            services.AddScoped<ITemplateRenderer, TemplateRenderer>();
       
            // EventBus konfigürasyonu
            services.AddSingleton(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    DefaultTopicName = "SocialAppEventBus",
                    SubscriberClientAppName = "NotificationService",
                    Connection = new ConnectionFactory()
                    {
                        HostName = "localhost",
                        Port = 5672,
                        UserName = "guest",
                        Password = "guest"
                    },
                    EventBusType = EventBusType.RabbitMQ,
                };

                return EventBusFactory.Create(config, sp);
            });

            services.AddHostedService<Worker>();

            return services;
        }

        public static void ConfigureNotificationServiceEventSubscriptions(this IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.Subscribe<SendVerificationCodeIntegrationEvent, SendVerificationCodeEventHandler>();
        }
    }
}
