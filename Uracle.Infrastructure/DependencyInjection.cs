

using Uracle.Application.Abstractions.Services;
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
            services.AddScoped<IJwtService, JWTService>();
            services.AddHttpClient<IStravaService, StravaService>();
            services.AddScoped<IContestRepository, ContestRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<ITeamMemberActivityRepository, TeamMemberActivityRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IUserTeamsCacheService, UserTeamsCacheService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(new AuditableEntityInterceptor());
            });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "_uracle";
            });
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.Configure<StravaOptions>(configuration.GetSection("Strava"));
            services.Configure<AuthCookieOptions>(
                                configuration.GetSection("AuthCookies"));
            return services;
        }
    }
}
