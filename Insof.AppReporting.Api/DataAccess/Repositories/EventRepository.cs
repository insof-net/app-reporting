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
        await using var connection = new MySqlConnection(_settings.ReportingConnectionString);

        var id = await connection.ExecuteScalarAsync<int>("InsertEvent",
            new
            {
                p_Timestamp = appEvent.Timestamp,
                p_Severity = (int)appEvent.Severity,
                p_SystemName = appEvent.SystemName,
                p_ApplicationName = appEvent.ApplicationName,
                p_ApplicationVersion = appEvent.ApplicationVersion,
                p_HostName = appEvent.HostName,
                p_FilePath = appEvent.FilePath,
                p_MethodName = appEvent.MethodName,
                p_ClassName = appEvent.ClassName,
                p_LineNumber = appEvent.LineNumber,
                p_Message = appEvent.Message,
                p_ExceptionType = appEvent.ExceptionType,
                p_ExceptionStackTrace = appEvent.ExceptionStackTrace
            }, commandType: CommandType.StoredProcedure);

        foreach (var attribute in appEvent.AppEventAttributes)
        {
            await connection.ExecuteAsync("InsertEventAttribute", new
            {
                p_Id = id,
                p_Name = attribute.Name,
                p_Value = attribute.Value
            }, commandType: CommandType.StoredProcedure);
        }
    }
}