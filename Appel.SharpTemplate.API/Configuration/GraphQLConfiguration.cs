using Appel.SharpTemplate.Infrastructure.GraphQL.Queries;
using Appel.SharpTemplate.Infrastructure.GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Appel.SharpTemplate.API.Configuration;

public static class GraphQLConfiguration
{
    public static IServiceCollection AddGraphQLConfiguration(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<UserType>();

        return services;
    }
}
