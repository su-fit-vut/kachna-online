// StartupExtensions.cs
// Author: Ondřej Ondryáš

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KachnaOnline.Data.Extensions
{
    public static class StartupExtensions
    {
        public static void AddAppDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(configuration.GetConnectionString("AppDb"),
                    mysqlOptions => mysqlOptions.MigrationsAssembly("KachnaOnline.Data"));
            });
        }
    }
}
