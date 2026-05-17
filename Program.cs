using CardsTools.Commands;
using CardsTools.Data.Managers;
using CardsTools.Hosting;
using CardsTools.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace CardsTools;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
            .CreateLogger();

        try
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Logging.ClearProviders();
            builder.Services.AddSerilog(Log.Logger);

            builder.Services
                .AddSingleton<IDeckStorage, JsonDeckStorage>()
                .AddSingleton<CardManager>()
                .AddSingleton<CommandRunner>()
                .AddHostedService<ConsoleAppService>();

            var host = builder.Build();
            await host.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "fatal error");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
