using System.Threading.Tasks;
using Insof.AppReporting.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Insof.AppReporting.Api.Startup;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services
            .AddServices()
            .ConfigureHttpJsonOptions(opt =>
            {
                opt.SerializerOptions.PropertyNameCaseInsensitive = true;
                opt.SerializerOptions.AllowTrailingCommas = true;
                opt.SerializerOptions.WriteIndented = true;
            })
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
        
        var app = builder.Build();

        app.MapEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        await app.RunAsync();
    }
}