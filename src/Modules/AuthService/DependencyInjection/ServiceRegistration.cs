using AuthService.Data;
using AuthService.Data.Repositories;
using AuthService.IntegrationEventHandlers;
using AuthService.Services;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Reflection;

namespace AuthService.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration baseConfiguration)
        {
            // 1. auth.settings.json yolunu belirle
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var authSettingsPath = Path.Combine(AppContext.BaseDirectory, $"auth.settings.{environment}.json");
            if (!File.Exists(authSettingsPath))
                throw new FileNotFoundException("auth.settings.json bulunamadı", authSettingsPath);

            // 2. auth.settings.json'u yükle
            var authConfiguration = new ConfigurationBuilder()
                .AddJsonFile(authSettingsPath, optional: false, reloadOnChange: true)
                .Build();

            // 3. gerekiyorsa global config ile birleştir (opsiyonel)
            var mergedConfiguration = new ConfigurationBuilder()
                .AddConfiguration(baseConfiguration)
                .AddConfiguration(authConfiguration)
                .Build();

            services.AddSingleton<IConfiguration>(mergedConfiguration);

            // 4. mergedConfiguration'dan config oku
            var connectionString = mergedConfiguration.GetConnectionString("DefaultConnection");

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
            });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ITokenService, TokenService>();     
            services.AddScoped<UserAccountSoftDeletedIntegrationEventHandler>();

            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>()
                .UseSqlServer(connectionString);

            using var context = new AuthDbContext(optionsBuilder.Options, null);
            context.Database.EnsureCreated();
            context.Database.Migrate();

            services.AddSingleton(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    DefaultTopicName = "SocialAppEventBus",
                    SubscriberClientAppName = "AuthService",
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

            return services;
        }

        public static void ConfigureAuthServiceEventSubscriptions(this IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserAccountSoftDeletedIntegrationEvent, UserAccountSoftDeletedIntegrationEventHandler>();
        }
    }
}
