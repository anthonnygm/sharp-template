using Microsoft.Extensions.DependencyInjection;

namespace Appel.SharpTemplate.Infrastructure.Configuration
{
    public static class CacheConfiguration
    {
        public static IServiceCollection AddCacheConfiguration(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            return services;
        }
    }
}
