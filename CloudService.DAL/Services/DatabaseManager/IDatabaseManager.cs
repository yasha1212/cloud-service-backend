using System;

namespace CloudService.DAL.Services.DatabaseManager
{
    public interface IDatabaseManager : IDisposable
    {
        void Migrate();
    }
}
