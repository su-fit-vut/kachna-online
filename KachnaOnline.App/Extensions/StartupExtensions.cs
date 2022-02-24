using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KachnaOnline.App.Swagger;
using KachnaOnline.Business.Configuration;
using KachnaOnline.Business.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog.AspNetCore;
using Serilog.Events;

namespace KachnaOnline.App.Extensions
{
    public static class StartupExtensions
    {
        public static void ConfigureSerilogRequestLogging(this IServiceCollection services)
        {
            services.Configure<RequestLoggingOptions>(options =>
            {
                options.EnrichDiagnosticContext = (diag, httpContext) =>
                {
                    if (httpContext == null)
                        return;

                    // Log HTTP Referer if there's one
                    if (httpContext.Request.Headers.TryGetValue("Referer", out var refererValues))
                    {
                        var host = httpContext.Request.Host.Host;
                        var referer = refererValues.Count > 0 ? refererValues[0] : null;
                        if (host != null && referer != null && !referer.Contains(host))
                            diag.Set("RequestReferer", referer);
                    }

                    // Log remote IP
                    diag.Set("RemoteIp", httpContext.Connection.RemoteIpAddress);
                };

                // Configures the event level based on the status code (only if there's no exception)
                options.GetLevel = (context, elapsed, exception) => exception == null
                    ? context.Response.StatusCode switch
                    {
                        // If the request takes more than 5 seconds to handle, there's something wrong
                        // and it is worth logging
                        <400 => elapsed > 5000 ? LogEventLevel.Warning : LogEventLevel.Debug,
                        404 => LogEventLevel.Debug,
                        <500 => LogEventLevel.Information,
                        _ => LogEventLevel.Warning
                    }
                    : LogEventLevel.Error;
            });
        }

        /// <summary>
        /// Configures <see cref="AuthenticationOptions"/> and <see cref="JwtBearerOptions"/> to enable authentication
        /// using a local-issued signed JWT.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">A <see cref="IConfiguration"/> to load the JWT configuration from.</param>
        public static void AddCustomJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration.GetSection("Jwt").Get<JwtOptions>().Secret;
            var jwtKeyBytes = Encoding.ASCII.GetBytes(jwtKey);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.SaveToken = false;
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
                        ValidAlgorithms = new[] { "HS256" },
                        NameClaimType = IdentityConstants.NameClaim,
                        RoleClaimType = IdentityConstants.RolesClaim
                    };
                });
        }

        /// <summary>
        /// Configures and adds a Swagger generator.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> to add services to.</param>
        public static void AddCustomSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.UseOneOfForPolymorphism();
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Kachna Online",
                        Version = "v1.0.0",
                        Contact = new OpenApiContact() { Email = "xondry02@su.fit.vutbr.cz", Name = "Ondřej Ondryáš" },
                    });
                c.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "Server-issued JWT Bearer token.",
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "JWTBearer"
                            }
                        },
                        new string[] { }
                    }
                });

                // Filter out references to the ProblemDetails scheme
                c.SchemaFilter<NullableFilter>();
                // Add authorization info to endpoints
                c.OperationFilter<AuthorizationFilter>();
                // Set 'nullable' to false in properties of certain schemes.
                c.OperationFilter<ProblemDetailsFilter>();
                // Load endpoint documentation from the XML generated during build from embedded xmldocs
                var filePath = Path.Combine(AppContext.BaseDirectory, "KachnaOnline.App.xml");
                c.IncludeXmlComments(filePath);
                filePath = Path.Combine(AppContext.BaseDirectory, "KachnaOnline.Dto.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        public static void UseUploadedImagesStaticFiles(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var imageDirectory = Path.Combine(env.ContentRootPath, ImageConstants.ImageDirectory);
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            // Serve uploaded images.
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(imageDirectory),
                RequestPath = ImageConstants.ImageUrlPath,
                HttpsCompression = HttpsCompressionMode.Compress,
                ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>()
                {
                    { ".jpg", "image/jpeg" }
                }),
                ServeUnknownFileTypes = false,
                OnPrepareResponse = ctx =>
                {
                    // using Microsoft.AspNetCore.Http;
                    ctx.Context.Response.Headers.Append(
                        "Cache-Control", $"public, max-age={ImageConstants.CacheMaxAge}");
                }
            });
        }
    }
}
