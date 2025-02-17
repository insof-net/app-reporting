using System;
using Insof.AppReporting.Client.Models;

namespace Insof.AppReporting.Client;

public sealed class AppReportingClientConfiguration
{
    public string SystemName { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string ApplicationVersion { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string ApiBaseAddress { get; set; } = string.Empty;
    public TimeSpan ApiTimeout { get; set; } = TimeSpan.Zero;
    internal Action<Exception, AppEvent>? OnError { get; set; }

    internal void Validate()
    {
        if (string.IsNullOrEmpty(SystemName))
            throw new ArgumentException(nameof(SystemName));

        if (string.IsNullOrEmpty(ApplicationName))
            throw new ArgumentException(nameof(ApplicationName));

        if (string.IsNullOrEmpty(ApplicationVersion))
            throw new ArgumentException(nameof(ApplicationVersion));

        if (string.IsNullOrEmpty(HostName))
            throw new ArgumentException(nameof(HostName));

        if (string.IsNullOrEmpty(ApiBaseAddress))
            throw new ArgumentException(nameof(ApiBaseAddress));

        if (ApiTimeout <= TimeSpan.Zero)
            throw new ArgumentException(nameof(ApiTimeout));
    }
}