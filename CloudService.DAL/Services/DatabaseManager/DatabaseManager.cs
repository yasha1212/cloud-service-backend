using Microsoft.EntityFrameworkCore;
using System;

namespace CloudService.DAL.Services.DatabaseManager
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly ApplicationDbContext context;

        public DatabaseManager(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Migrate()
        {
            context.Database.Migrate();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
