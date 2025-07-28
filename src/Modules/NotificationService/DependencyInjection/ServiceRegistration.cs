using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Events;
using NotificationService.Events.Handlers;
using NotificationService.Services;
using EventBus.Factory;
using Microsoft.Extensions.Options;
using EventBus.Base;

namespace NotificationService.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddNotificationService(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                var config = sp.GetRequiredService<IOptions<EventBusConfig>>().Value;
                return EventBusFactory.Create(config, sp);
            });

            services.AddTransient<IIntegrationEventHandler<SendNotificationEvent>, SendNotificationEventHandler>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<ITemplateRenderer, TemplateRenderer>();

            return services;
        }
    }
}
