using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Reflection;
using UserService.Data;
using UserService.Data.Repositories;
using UserService.IntegrationEventHandlers;

namespace UserService.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddUserService(this IServiceCollection services, IConfiguration baseConfiguration)
        {
            // 1. user.settings.json yolunu belirle
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var userSettingsPath = Path.Combine(AppContext.BaseDirectory, $"user.settings.{environment}.json");
            if (!File.Exists(userSettingsPath))
                throw new FileNotFoundException("user.settings.json bulunamadı", userSettingsPath);

            // 2. user.settings.json'u yükle
            var userConfiguration = new ConfigurationBuilder()
                .AddJsonFile(userSettingsPath, optional: false, reloadOnChange: true)
                .Build();

            // 3. gerekiyorsa global config ile birleştir (opsiyonel)
            var mergedConfiguration = new ConfigurationBuilder()
                .AddConfiguration(baseConfiguration)
                .AddConfiguration(userConfiguration)
                .Build();

            services.AddSingleton<IConfiguration>(mergedConfiguration);

            // 4. mergedConfiguration'dan config oku
            var connectionString = mergedConfiguration.GetConnectionString("DefaultConnection");
           
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
            });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<UserRegisteredIntegrationEventHandler>();

            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>()
                .UseSqlServer(connectionString);

            using var context = new UserDbContext(optionsBuilder.Options, null);
            context.Database.EnsureCreated();
            context.Database.Migrate();

            services.AddSingleton(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    DefaultTopicName = "SocialAppEventBus",
                    SubscriberClientAppName = "UserService",
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

        public static void ConfigureUserServiceEventSubscriptions(this IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();         
            eventBus.Subscribe<UserRegisteredIntegrationEvent, UserRegisteredIntegrationEventHandler>();
        }
    }
}
