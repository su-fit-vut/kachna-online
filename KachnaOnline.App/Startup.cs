// Startup.cs
// Author: Ondřej Ondryáš

using System;
using KachnaOnline.App.Extensions;
using KachnaOnline.Data;
using KachnaOnline.Data.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace KachnaOnline.App
{
    /// <summary>
    /// ASP.NET Core Startup class that configures the app's services container and request pipeline.
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Configures the app's dependency injection container.
        /// </summary>
        /// <param name="services">A service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configures custom rules for Serilog's request logging.
            services.ConfigureSerilogRequestLogging();

            // Add scoped database context.
            services.AddAppDatabase(this.Configuration);

            // Add MVC controllers.
            services.AddControllers();

            // Add OpenAPI document service.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "KachnaOnline", Version = "v1" });
            });
        }

        /// <summary>
        /// Configures the request processing pipeline.
        /// </summary>
        /// <param name="app">An application builder.</param>
        /// <param name="env">
        /// An <see cref="IWebHostEnvironment" /> instance that contains information about the current
        /// environment.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Use the ErrorController to handle unhandled exceptions.
                app.UseExceptionHandler("/error");

                // Add the Strict-Transport-Security header.
                app.UseHsts();
            }

            // Add Serilog's request logging middleware.
            app.UseSerilogRequestLogging();

            // Redirect all HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Serve client app.
            app.UseStaticFiles();

            // Handle 400+ HTTP status codes.
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            // Add OpenAPI document providing middleware.
            app.UseSwagger();

            // Add SwaggerUI.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kachna Online API"));

            // Add routing middleware.
            app.UseRouting();

            // Add authorization middleware.
            app.UseAuthorization();

            // Map controller endpoints using the default mapping strategy.
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            // Run migrations (optionally).
            if (Environment.CommandLine.Contains("--migrate-db"))
            {
                dbContext.Database.Migrate();
            }

            // Add initial data (optionally).
            if (Environment.CommandLine.Contains("--bootstrap-db"))
            {
                var dbBootstrapper = new DataBootstrapper(dbContext);
                dbBootstrapper.BootstrapDatabase();
            }
        }
    }
}
