using System;
using KachnaOnline.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace KachnaOnline.App
{
    /// <summary>
    /// The app's entry point class.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                InitSerilog();

                var migrateDb = Environment.CommandLine.Contains("--migrate-db");
                var bootstrapDb = Environment.CommandLine.Contains("--bootstrap-db");

                if (migrateDb || bootstrapDb)
                {
                    Log.Information("Database initialization mode.");

                    var builder = CreateHostBuilder(args).Build();
                    using var scope = builder.Services.CreateScope();
                    using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (migrateDb)
                    {
                        Log.Information("Applying database migrations.");
                        dbContext.Database.Migrate();
                    }

                    if (bootstrapDb)
                    {
                        Log.Information("Bootstrapping the database.");
                        var dbBootstrapper = new DataBootstrapper(dbContext);
                        dbBootstrapper.BootstrapDatabase();
                    }

                    Log.Information("The requested operations were completed, shutting down.");
                    return;
                }

                Log.Information("Starting web host.");
                CreateHostBuilder(args).Build().Run();
            }
            finally
            {
                // Always flush buffered logs.
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Creates a temporary (bootstrap) logger that is replaced with a fully configured one when
        /// a service container is initialized (after the app host has been initialized).
        /// </summary>
        private static void InitSerilog()
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            Log.Logger = logger;
        }

        /// <summary>
        /// Creates an ASP.NET web host.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>An <see cref="IHostBuilder"/> instance configured using the <see cref="Startup"/> class.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, config) => config
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}
