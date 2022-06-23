using CloudService.DAL;
using CloudService.Entities;

namespace CloudService.Impl.Services.Files
{
    public class FilesService : BaseService, IFilesService
    {
        public FilesService(ApplicationDbContext dbContext) 
            : base(dbContext)
        {
        }
    }
}
