using CloudService.DAL;
using CloudService.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Folders
{
    public class FoldersService : BaseService, IFoldersService
    {
        public FoldersService(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
