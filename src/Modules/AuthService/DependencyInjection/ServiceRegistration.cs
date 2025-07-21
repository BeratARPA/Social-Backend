using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            //services.AddScoped<IUserRepository, UserRepository>();

            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>()
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            using var context = new AuthDbContext(optionsBuilder.Options, null);
            context.Database.EnsureCreated();
            context.Database.Migrate();

            return services;
        }
    }
}
