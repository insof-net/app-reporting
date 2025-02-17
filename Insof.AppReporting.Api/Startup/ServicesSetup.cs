using System;
using Insof.AppReporting.Api.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Insof.AppReporting.Api.Startup;

public static class ServicesSetup
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var settings = GetSettings();

        services
            .AddSingleton<IEventRepository, EventRepository>()
            .AddSingleton(settings);

        return services;
    }
    
    private static Settings GetSettings()
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile($"appsettings.{environmentName}.json", true)
            .AddEnvironmentVariables()
            .Build();

        return config.Get<Settings>()
               ?? throw new Exception("Unable to load ConfigurationSettings");
    }
}