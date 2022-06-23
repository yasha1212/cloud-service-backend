using CloudService.DAL;
using CloudService.Entities;

namespace CloudService.Impl.Services.Extensions
{
    public class ExtensionsService : BaseService, IExtensionsService
    {
        public ExtensionsService(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
