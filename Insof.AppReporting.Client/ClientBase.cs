using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Insof.AppReporting.Client.Response;

namespace Insof.AppReporting.Client;

public abstract class ClientBase
{
    protected const string MediaType = "application/json";
    protected readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private readonly IHttpClientFactory _httpClientFactory;

    protected ClientBase(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    protected async Task<T> SendAsync<T>(HttpRequestMessage request) where T : ResponseBase, new()
    {
        var httpClient = _httpClientFactory.CreateClient(nameof(AppReportingClient));
        using var response = await httpClient.SendAsync(request).ConfigureAwait(false);
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var r = JsonSerializer.Deserialize<T>(stream, JsonSerializerOptions);
            if (r == null)
            {
                return new T
                {
                    StatusCode = (int)response.StatusCode,
                    Message = "Unable to deserialize response"
                };
            }
            r.StatusCode = (int)response.StatusCode;
            return r;
        }
        catch (JsonException ex)
        {
            var res = new T
            {
                Message = ex.Message,
                StatusCode = (int)response.StatusCode,
                Exception = ex
            };
            return res;
        }
    }
}