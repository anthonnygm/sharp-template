using Microsoft.Extensions.DependencyInjection;

namespace Appel.SharpTemplate.API.Configuration
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
