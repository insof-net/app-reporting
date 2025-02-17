using System;
using System.Threading.Tasks;
using Insof.AppReporting.Api.DataAccess.Repositories;
using Insof.AppReporting.Client.Models;
using Insof.AppReporting.Client.Response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Insof.AppReporting.Api.Endpoints;

internal static class LogEventEndpoint
{
    internal static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/log-event", LogEventAsync)
            .WithName("Log-Event")
            .WithDisplayName("Log Event")
            .WithOpenApi();
        
        return routes;
    }

    private static async Task<IResult> LogEventAsync(
        AppEvent appEvent,
        IEventRepository eventRepository)
    {
        try
        {
            await eventRepository.InsertEvent(appEvent);
            return TypedResults.Ok(new LogEventResponse { Message = "OK" });
        }
        catch (Exception)
        {
            return TypedResults.StatusCode(500);
        }
    }
}