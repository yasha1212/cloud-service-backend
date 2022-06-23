using CloudService.DAL;
using CloudService.Impl.Services.Folders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Storage
{
    public class StorageService : BaseService, IStorageService
    {
        private const long DEFAULT_CAPACITY = 100000000;

        private readonly IFoldersService foldersService;

        public StorageService(
            ApplicationDbContext dbContext,
            IFoldersService foldersService
            )
            : base(dbContext)
        {
            this.foldersService = foldersService;
        }

        // ADD SAVING_SERVICE
        public async Task<Entities.Storage> Create(string name, string userId)
        {
            var model = new Entities.Storage
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                UserId = userId,
                Capacity = DEFAULT_CAPACITY,
                UsedCapacity = 0,
                CreatedOn = DateTime.Now
            };

            DbContext.Storages.Add(model);

            await DbContext.SaveChangesAsync();

            await foldersService.CreateRoot(model.Id);

            return model;
        }

        public async Task<Entities.Storage> Get(string userId)
        {
            var model = await DbContext.Storages
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == userId);

            return model;
        }

        public async Task<Entities.Storage> Update(string name, string userId)
        {
            var model = await DbContext.Storages
                .FirstOrDefaultAsync(e => e.UserId == userId);

            model.Name = name;

            DbContext.Storages.Update(model);

            await DbContext.SaveChangesAsync();

            return model;
        }
    }
}
