namespace Todo.Infrastructure;

using System.Data;
using Application.Interfaces;
using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Repositories;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDbConnection>(sp =>
            new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddScoped<IDapperExecutor, DapperExecutor>();
        services.AddScoped<IListRepository, ListRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}