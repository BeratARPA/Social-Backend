using EventBus.Base;
using EventBus.Factory;
using NotificationService.Worker;
using NotificationService.Worker.Events.Handlers;
using NotificationService.Worker.Services;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

// Handler'larý Scoped olarak kaydet
builder.Services.AddScoped<SendVerificationCodeEventHandler>();

// Service'leri kaydet
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ISmsSender, SmsSender>();
builder.Services.AddScoped<ITemplateRenderer, TemplateRenderer>();

// EventBus konfigürasyonu
builder.Services.AddSingleton(sp =>
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

// Worker service'i kaydet
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
