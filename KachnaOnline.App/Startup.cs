using System.Collections.Generic;
using System.Linq;
using System.Net;
using KachnaOnline.App.Configuration;
using KachnaOnline.App.DateHandling;
using KachnaOnline.App.Extensions;
using KachnaOnline.Business.Extensions;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using KachnaOnline.Business.Data.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace KachnaOnline.App
{
    /// <summary>
    /// ASP.NET Core Startup class that configures the app's services container and request pipeline.
    /// </summary>
    public class Startup
    {
        public const string LocalCorsPolicy = "LocalPolicy";
        public const string MainCorsPolicy = "MainPolicy";

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
            // Add CORS policy for local development and internal apps
            services.AddCors(o =>
            {
                o.AddPolicy(LocalCorsPolicy, builder =>
                {
                    builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
                    builder.WithOrigins("https://localhost:4200").AllowAnyHeader().AllowAnyMethod();
                    builder.WithOrigins("https://su-int.fit.vutbr.cz").AllowAnyHeader().AllowAnyMethod();
                });

                o.AddPolicy(MainCorsPolicy, builder =>
                {
                    builder.WithOrigins("https://su.fit.vut.cz", "https://su.fit.vutbr.cz",
                            "https://www.su.fit.vut.cz", "https://www.su.fit.vutbr.cz",
                            "https://su-int.fit.vutbr.cz", "https://su-dev.fit.vutbr.cz")
                        .AllowAnyHeader().AllowAnyMethod();
                });
            });

            // Load configuration objects.
            services.Configure<KisOptions>(this.Configuration.GetSection("Kis"));
            services.Configure<JwtOptions>(this.Configuration.GetSection("Jwt"));
            services.Configure<ClubStateOptions>(this.Configuration.GetSection("States"));
            services.Configure<BoardGamesOptions>(this.Configuration.GetSection("BoardGames"));
            services.Configure<PushOptions>(this.Configuration.GetSection("Push"));
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

                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.Converters.Add(new CustomDateTimeConverter());
                })
                .AddMvcOptions(options =>
                {
                    var inputFormatter = options.InputFormatters.OfType<NewtonsoftJsonInputFormatter>().First();
                    inputFormatter.SupportedMediaTypes.Clear();
                    inputFormatter.SupportedMediaTypes.Add("application/json");
                    inputFormatter.SupportedMediaTypes.Add("text/json");

                    options.ModelBinderProviders.Insert(0, new KindAdjustingDateTimeModelBinderProvider());
                });

            // Add JWT authentication.
            services.AddCustomJwtAuthentication(this.Configuration);

            // Add custom authorization policies.
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthConstants.AnyManagerPolicy, policy =>
                    policy.RequireRole(AuthConstants.Admin, AuthConstants.EventsManager, AuthConstants.StatesManager,
                        AuthConstants.BoardGamesManager));
                options.AddPolicy(AuthConstants.AdminOrBoardGamesManagerPolicy, policy =>
                    policy.RequireRole(AuthConstants.Admin, AuthConstants.BoardGamesManager));
            });

            // Add OpenAPI document service.
            services.AddCustomSwaggerGen();

            // Add reverse proxy forwarded header options (if configured)
            var servingConfig = this.Configuration.GetSection("Serving").Get<ServingConfiguration>();
            if (servingConfig.ReverseProxyNetworkIp != null)
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.All;
                    options.KnownNetworks.Add(new IPNetwork(
                        IPAddress.Parse(servingConfig.ReverseProxyNetworkIp), servingConfig.ReverseProxyNetworkPrefix));
                });
            }
        }

        /// <summary>
        /// Configures the request processing pipeline.
        /// </summary>
        /// <param name="app">An application builder.</param>
        /// <param name="env">
        /// An <see cref="IWebHostEnvironment" /> instance that contains information about the current
        /// environment.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var servingConfig = this.Configuration.GetSection("Serving").Get<ServingConfiguration>();

            // Use forwarded headers if a reverse proxy network is configured.
            if (servingConfig.ReverseProxyNetworkIp != null)
            {
                app.UseForwardedHeaders();
            }

            // Normally, we don't want the app to serve static files (i.e. the frontend).
            // However, it might be useful for development, so the app can be configured to serve wwwroot
            // on any path. This is intentionally put before UsePathBase.
            if (servingConfig.ServeStaticFiles)
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    RequestPath = new PathString(servingConfig.StaticFilesPathBase),
                    FileProvider = new PhysicalFileProvider(env.WebRootPath)
                });
            }

            // Use path base if configured.
            if (servingConfig.PathBase != null)
            {
                Log.Information("Using PathBase: {PathBase}", servingConfig.PathBase);
                app.UsePathBase(new PathString(servingConfig.PathBase));
            }

            if (env.IsDevelopment())
            {
                // Use developer exceptions in development.
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

            // Handle 400+ HTTP status codes.
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            // Add OpenAPI document providing middleware.
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((doc, req) =>
                {
                    doc.Servers = new List<OpenApiServer>()
                    {
                        new() { Url = servingConfig.PathBase ?? "/" }
                    };
                });
            });

            // Add SwaggerUI.
            app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Kachna Online API"));

            // Add routing middleware.
            app.UseRouting();

            app.UseCors(env.IsDevelopment() ? LocalCorsPolicy : MainCorsPolicy);

            // Add authorization middleware.
            app.UseAuthentication();
            app.UseAuthorization();

            // Map controller endpoints using the default mapping strategy.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                if (env.IsDevelopment())
                {
                    endpoints.MapFallbackToFile("index.html");
                }
            });
        }
    }
}
