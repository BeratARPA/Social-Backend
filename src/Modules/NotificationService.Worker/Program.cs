using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using NotificationService.Worker;
using NotificationService.Worker.Events.Handlers;
using NotificationService.Worker.Services;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<SendVerificationCodeEventHandler>();

builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<ISmsSender, SmsSender>();
builder.Services.AddSingleton<ITemplateRenderer, TemplateRenderer>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
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

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
