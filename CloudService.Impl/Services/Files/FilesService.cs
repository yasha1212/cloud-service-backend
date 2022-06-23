using CloudService.DAL;
using CloudService.Entities;
using CloudService.Impl.Services.Folders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Files
{
    public class FilesService : BaseService, IFilesService
    {
        private readonly IFoldersService foldersService;

        public FilesService(
            ApplicationDbContext dbContext,
            IFoldersService foldersService
            ) 
            : base(dbContext)
        {
            this.foldersService = foldersService;
        }

        // ADD SAVING_SERVICE

        public async Task Create(string name, string parentId)
        {
            var parent = await foldersService.Get(parentId);

            var model = new FileInfo
            {
                Id = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                ExtensionId = "7a7b36d3-533d-44c4-9cb8-71db38ab5c8b", //CHANGE
                FolderId = parentId,
                IsProtected = true,
                Name = name,
                Size = 1200, //CHANGE
                StorageId = parent.StorageId,
                Path = $"{parent.Path}\\{name}.docx"
            };

            DbContext.FileInfos.Add(model);

            await DbContext.SaveChangesAsync();

            await foldersService.UpdateSize(parentId, model.Size);
        }

        public async Task Delete(string id)
        {
            var model = await DbContext.FileInfos
                .FirstOrDefaultAsync(e => e.Id == id);

            DbContext.FileInfos.Remove(model);

            await DbContext.SaveChangesAsync();

            await foldersService.UpdateSize(model.FolderId, -model.Size);
        }

        public async Task<FileInfo> Get(string id)
        {
            var model = await DbContext.FileInfos
                .AsNoTracking()
                .Include(e => e.Extension)
                .Include(e => e.Storage)
                .FirstOrDefaultAsync(e => e.Id == id);

            return model;
        }

        public async Task<IEnumerable<FileInfo>> GetAll(string parentId)
        {
            var files = await DbContext.FileInfos
                .Include(e => e.Extension)
                .AsNoTracking()
                .Where(e => e.FolderId == parentId)
                .ToListAsync();

            return files;
        }

        public async Task<IEnumerable<FileInfo>> GetAllPublished(string storageId)
        {
            var files = await DbContext.FileInfos
                .Include(e => e.Extension)
                .AsNoTracking()
                .Where(e => e.StorageId == storageId && !e.IsProtected)
                .ToListAsync();

            return files;
        }

        public async Task Update(string id, string name)
        {
            var model = await DbContext.FileInfos
                .Include(e => e.Folder)
                .Include(e => e.Extension)
                .FirstOrDefaultAsync(e => e.Id == id);

            model.Name = name;
            model.Path = $"{model.Folder.Path}\\{name}{model.Extension.Format}";
            model.LastModifiedOn = DateTime.Now;

            DbContext.FileInfos.Update(model);

            await DbContext.SaveChangesAsync();
        }
    }
}
