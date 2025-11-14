

using Uracle.Infrastructure.Options;

namespace Uracle.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
        
        {
            var connectionString = configuration.GetConnectionString("Database");

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IStravaService, StravaService>();    
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
            });

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.Configure<StravaOptions>(configuration.GetSection("Strava"));

            return services;
        }
    }
}
