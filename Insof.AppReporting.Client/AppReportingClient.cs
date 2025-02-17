using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Insof.AppReporting.Client.Helpers;
using Insof.AppReporting.Client.Models;
using Insof.AppReporting.Client.Response;

namespace Insof.AppReporting.Client;

public interface IAppReportingClient
{
    Task<LogEventResponse> LogEventAsync<T>(SeverityLevel severityLevel,
        string message,
        object? attributes = null,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) where T : class;

    LogEventResponse LogEvent<T>(SeverityLevel severityLevel,
        string message,
        object? attributes = null,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) where T : class;

    Task<LogEventResponse> LogEventAsync(Type type,
        SeverityLevel severityLevel,
        string message,
        object? attributes = null,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);

    LogEventResponse LogEvent(Type type,
        SeverityLevel severityLevel,
        string message,
        object? attributes = null,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);
}

public sealed class AppReportingClient : ClientBase, IAppReportingClient
{
    private readonly ApplicationInformation _applicationInformation;

    public AppReportingClient(ApplicationInformation applicationInformation,
        IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
        _applicationInformation = applicationInformation;
    }

    public async Task<LogEventResponse> LogEventAsync(Type type,
        SeverityLevel severityLevel,
        string message,
        object? attributes = null,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var appEvent = new AppEvent
        {
            Severity = severityLevel,
            SystemName = _applicationInformation.System,
            ApplicationName = _applicationInformation.ApplicationName,
            ApplicationVersion = _applicationInformation.ApplicationVersion,
            HostName = _applicationInformation.HostName,
            Timestamp = DateTimeOffset.UtcNow,
            Message = message,
            AppEventAttributes = attributes?.MapAttributes() ?? [],
            ExceptionType = exception?.GetType().FullName ?? string.Empty,
            ExceptionStackTrace = exception?.StackTrace ?? string.Empty,
            MethodName = method,
            ClassName = type.Name,
            FilePath = filePath,
            LineNumber = lineNumber
        };

        try
        {
            var jsonPayload = JsonSerializer.Serialize(appEvent, JsonSerializerOptions);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, MediaType);
            using var httpRequest = new HttpRequestMessage();
            httpRequest.Method = HttpMethod.Post;
            httpRequest.Content = content;
            httpRequest.RequestUri = new Uri("event", UriKind.Relative);
            var r = await SendAsync<LogEventResponse>(httpRequest)
                .ConfigureAwait(false);

            if (r.Exception != null)
                throw r.Exception;

            return r;
        }
        catch (Exception ex)
        {
            _applicationInformation.OnError?.Invoke(ex, appEvent);

            return new LogEventResponse
            {
                StatusCode = -1,
                Message = ex.Message
            };
        }
    }

    public LogEventResponse LogEvent(Type type,
        SeverityLevel severityLevel,
        string message,
        object? attributes = null,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        return AsyncHelper.RunSync(async () => await LogEventAsync(type,
            severityLevel,
            message,
            attributes,
            exception,
            method,
            // ReSharper disable once ExplicitCallerInfoArgument
            filePath,
            // ReSharper disable once ExplicitCallerInfoArgument
            lineNumber));
    }


    public async Task<LogEventResponse> LogEventAsync<T>(SeverityLevel severityLevel,
        string message,
        object? attributes = null,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) where T : class
    {
        return await LogEventAsync(typeof(T),
            severityLevel,
            message,
            attributes,
            exception,
            method,
            // ReSharper disable once ExplicitCallerInfoArgument
            filePath,
            // ReSharper disable once ExplicitCallerInfoArgument
            lineNumber);
    }

    public LogEventResponse LogEvent<T>(SeverityLevel severityLevel,
        string message,
        object? attributes = null,
        Exception? exception = null,
        [CallerMemberName] string method = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0) where T : class
    {
        return AsyncHelper.RunSync(async () => await LogEventAsync<T>(severityLevel,
            message,
            attributes,
            exception,
            method,
            // ReSharper disable once ExplicitCallerInfoArgument
            filePath,
            // ReSharper disable once ExplicitCallerInfoArgument
            lineNumber));
    }
}