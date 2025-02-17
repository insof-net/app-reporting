using System;

namespace Insof.AppReporting.Client.Response;

public abstract class ResponseBase
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
}