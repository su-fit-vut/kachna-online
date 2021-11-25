// Startup.cs
// Author: Ondřej Ondryáš

using System;
using System.Linq;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Data.Extensions;
using KachnaOnline.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
            // Load configuration objects.
            services.Configure<KisOptions>(this.Configuration.GetSection("Kis"));
            services.Configure<JwtOptions>(this.Configuration.GetSection("Jwt"));
            services.Configure<ClubStateOptions>(this.Configuration.GetSection("States"));
            services.Configure<BoardGamesOptions>(this.Configuration.GetSection("BoardGames"));
            services.Configure<MailOptions>(this.Configuration.GetSection("Mail"));
            services.Configure<EventsOptions>(this.Configuration.GetSection("Events"));

            // Configures custom rules for Serilog's request logging.
            services.ConfigureSerilogRequestLogging();

            // Add scoped database context.
            services.AddAppData(this.Configuration);

            // Add business layer services.
            services.AddBusinessLayer(this.Configuration);

            // Add MVC controllers.
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddMvcOptions(options =>
                {
                    var inputFormatter = options.InputFormatters.OfType<NewtonsoftJsonInputFormatter>().First();
                    inputFormatter.SupportedMediaTypes.Clear();
                    inputFormatter.SupportedMediaTypes.Add("application/json");
                    inputFormatter.SupportedMediaTypes.Add("text/json");
                });

            // Add JWT authentication.
            services.AddCustomJwtAuthentication(this.Configuration);

            // Add custom authorization policies.
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthConstants.AnyManagerPolicy, policy =>
                    policy.RequireRole(AuthConstants.Admin, AuthConstants.EventsManager, AuthConstants.StatesManager,
                        AuthConstants.BoardGamesManager));
            });

            // Add OpenAPI document service.
            services.AddCustomSwaggerGen();
        }

        /// <summary>
        /// Configures the request processing pipeline.
        /// </summary>
        /// <param name="app">An application builder.</param>
        /// <param name="env">
        /// An <see cref="IWebHostEnvironment" /> instance that contains information about the current
        /// environment.
        /// </param>
        /// <param name="dbContext">An <see cref="AppDbContext"/> database context instance.</param>
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

            if (!env.IsDevelopment())
            {
                // Redirect all HTTP requests to HTTPS.
                app.UseHttpsRedirection();
            }

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
            app.UseAuthentication();
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
