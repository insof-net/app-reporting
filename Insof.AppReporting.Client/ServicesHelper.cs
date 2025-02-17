using System;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using Insof.AppReporting.Client.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Insof.AppReporting.Client;

// ReSharper disable once UnusedType.Global
public static class ServicesHelper
{
    // ReSharper disable once UnusedMember.Global
    public static IServiceCollection ConfigureAppReportingClient(this IServiceCollection services,
        Action<AppReportingClientConfiguration> config,
        Action<Exception, AppEvent>? onError = null)
    {
        ArgumentNullException.ThrowIfNull(config);

        var callingAssembly = Assembly.GetCallingAssembly();

        // default values
        var configuration = new AppReportingClientConfiguration
        {
            ApiTimeout = TimeSpan.FromMilliseconds(500),
            HostName = Dns.GetHostName(),
            ApplicationVersion = callingAssembly.GetInformationalVersionString(),
            ApplicationName = callingAssembly.GetName().Name ?? string.Empty,
            OnError = onError
        };

        // get custom values during setup
        config.Invoke(configuration);

        // make sure they're valid
        configuration.Validate();

        // wire up everything
        services
            .AddSingleton<IAppReportingClient, AppReportingClient>()
            .AddSingleton(new ApplicationInformation
            {
                ApplicationName = configuration.ApplicationName,
                System = configuration.SystemName,
                ApplicationVersion = configuration.ApplicationVersion,
                HostName = configuration.HostName,
                OnError = configuration.OnError
            })
            .AddHttpClient(nameof(AppReportingClient), client =>
            {
                client.BaseAddress = new Uri(configuration.ApiBaseAddress);
                client.Timeout = configuration.ApiTimeout;
                client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

        return services;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static string GetInformationalVersionString(this Assembly assembly)
    {
        var version = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (string.IsNullOrEmpty(version))
            version = assembly.GetName().Version?.ToString() ?? string.Empty;

        return version;
    }
}