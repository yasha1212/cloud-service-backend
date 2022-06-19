using CloudService.DAL;

namespace CloudService.Impl
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
