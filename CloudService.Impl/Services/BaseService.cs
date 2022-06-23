using CloudService.DAL;

namespace CloudService.Impl.Services
{
    public abstract class BaseService
    {
        protected ApplicationDbContext DbContext { get; }

        protected BaseService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
