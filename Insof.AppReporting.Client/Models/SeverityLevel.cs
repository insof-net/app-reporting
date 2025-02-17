using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global

namespace Insof.AppReporting.Client.Models;

[JsonConverter(typeof(JsonStringEnumConverter<SeverityLevel>))]
public enum SeverityLevel
{
    Informational = 1,
    Benign = 2,
    Warning = 3,
    Error = 4,
    Critical = 5
}