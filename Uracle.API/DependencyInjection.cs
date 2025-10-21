namespace Uracle.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddHealthChecks();

            // Add API related services here
            return services;
        }

        public static WebApplication UseApiServices(this WebApplication app)
        {

            app.MapCarter();
            app.UseExceptionHandler(option => { });
            app.UseHealthChecks("/health",
                new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            return app;
        }
    }
}
