namespace Todo.CompositionRoot;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application;
using Infrastructure;

public static class CompositionRootServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationAndInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // useCases
        services.AddApplication();

        // DB
        services.AddInfrastructure(configuration);

        return services;
    }
}