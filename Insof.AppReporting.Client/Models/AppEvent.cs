using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Insof.AppReporting.Client.Models;

public sealed class AppEvent
{
    public required SeverityLevel Severity { get; init; }
    public required string SystemName { get; init; }
    public required string ApplicationName { get; init; }
    public required string ApplicationVersion { get; init; } = string.Empty;
    public required string HostName { get; init; } = string.Empty;
    public required DateTimeOffset Timestamp { get; init; }
    public required string FilePath { get; init; }
    public required string MethodName { get; init; }
    public required string ClassName { get; init; }
    public required string Message { get; init; }
    public required string ExceptionType { get; init; }
    public required string ExceptionStackTrace { get; init; }
    public required int LineNumber { get; init; }
    public required List<AppEventAttribute>? AppEventAttributes { get; init; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}