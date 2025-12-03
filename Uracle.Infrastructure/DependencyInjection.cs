

using Uracle.Infrastructure.Interceptors;
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
            services.AddScoped<IStravaRepository, StravaRepository>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddHttpClient<IStravaService, StravaService>();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(new AuditableEntityInterceptor());
            });

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.Configure<StravaOptions>(configuration.GetSection("Strava"));
            services.Configure<AuthCookieOptions>(
                                configuration.GetSection("AuthCookies"));
            return services;
        }
    }
}
