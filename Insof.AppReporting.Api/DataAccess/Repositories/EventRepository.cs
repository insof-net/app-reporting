using System.Data;
using System.Threading.Tasks;
using Dapper;
using Insof.AppReporting.Api.Startup;
using Insof.AppReporting.Client.Models;
using MySqlConnector;

namespace Insof.AppReporting.Api.DataAccess.Repositories;

public interface IEventRepository
{
    Task InsertEvent(AppEvent appEvent);
}

public class EventRepository : IEventRepository
{
    private readonly Settings _settings;

    public EventRepository(Settings settings)
    {
        _settings = settings;
    }
    
    public async Task InsertEvent(AppEvent appEvent)
    {
        await using var connection = new MySqlConnection(_settings.ConnectionString);

        var id = await connection.ExecuteScalarAsync<int>("todo:",
            new
            {
                appEvent.Timestamp,
                Severity = (int)appEvent.Severity,
                appEvent.SystemName,
                appEvent.ApplicationName,
                appEvent.ApplicationVersion,
                appEvent.HostName,
                appEvent.FilePath,
                appEvent.MethodName,
                appEvent.ClassName,
                appEvent.LineNumber,
                appEvent.Message,
                appEvent.ExceptionType,
                appEvent.ExceptionStackTrace
            },
            commandType: CommandType.StoredProcedure);

        foreach (var attribute in appEvent.AppEventAttributes)
        {
            await connection.ExecuteAsync("todo:", new
            {
                EventId = id,
                attribute.Name,
                attribute.Value
            });
        }
    }
}