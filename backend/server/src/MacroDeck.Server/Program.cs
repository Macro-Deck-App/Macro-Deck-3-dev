using MacroDeck.Server.Extensions;
using MacroDeck.Server.Infrastructure.Utils;
using Serilog;

namespace MacroDeck.Server;

public static class Program
{
    public static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

        DatabaseMigrationHelper.MigrateDatabase();

        await Host.CreateDefaultBuilder(args)
            .ConfigureSerilog()
            .ConfigureWebHostDefaults(hostBuilder =>
            {
                hostBuilder.UseStartup<Startup>();
                hostBuilder.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(8191);
                });
            })
            .Build()
            .RunAsync();
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Logger.Fatal(e.ExceptionObject as Exception,
            "Unhandled exception {Terminating}",
            e.IsTerminating ? "Terminating" : "Not terminating");
    }
}