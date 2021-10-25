// StartupExtensions.cs
// Author: Ondřej Ondryáš

using KachnaOnline.Business.Data.Repositories;
using KachnaOnline.Business.Data.Repositories.Abstractions;
using KachnaOnline.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KachnaOnline.Business.Data.Extensions
{
    public static class StartupExtensions
    {
        public static void AddAppData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(configuration.GetConnectionString("AppDb"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("AppDb")),
                    mysqlOptions => mysqlOptions.MigrationsAssembly("KachnaOnline.Data"));
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
