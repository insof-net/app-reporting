namespace Insof.AppReporting.Client.Models;

public sealed class AppEventAttribute
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}