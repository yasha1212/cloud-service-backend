using CloudService.DAL.Services.DatabaseManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CloudService.DAL.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var manager = scope.ServiceProvider.GetRequiredService<IDatabaseManager>())
                {
                    try
                    {
                        manager.Migrate();
                    }
                    catch (Exception e)
                    {
                        scope.ServiceProvider.GetRequiredService<ILogger<IDatabaseManager>>().LogError(e.ToString());
                    }
                }
            }

            return host;
        }
    }
}
