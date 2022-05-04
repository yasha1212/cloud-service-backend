using CloudService.DAL.Services.DatabaseManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CloudService.DAL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, Action<DataAccessOptions> optionsAction)
        {
            DataAccessOptions options = new DataAccessOptions();
            optionsAction.Invoke(options);

            services
                .AddDbContext<ApplicationDbContext>(dbOptions => dbOptions.UseSqlServer(options.ConnectionString))
                .AddScoped<IDatabaseManager, DatabaseManager>();

            return services;
        }
    }
}
