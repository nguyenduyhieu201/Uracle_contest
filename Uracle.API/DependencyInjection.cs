using SharedKernel.Exceptions.Handler;

namespace Uracle.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddExceptionHandler<CustomExceptionHandler>();
            // Add API related services here
            return services;
        }
    }
}
