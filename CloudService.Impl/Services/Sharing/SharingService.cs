using CloudService.DAL;
using CloudService.Entities;

namespace CloudService.Impl.Services.Sharing
{
    public class SharingService : BaseService, ISharingService
    {
        public SharingService(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
