using System;

namespace Insof.AppReporting.Client.Models;

public sealed class ApplicationInformation
{
    public required string System { get; init; }
    public required string ApplicationName { get; init; }
    public required string ApplicationVersion { get; init; }
    public required string HostName { get; init; }
    internal Action<Exception, AppEvent>? OnError { get; init; }
}