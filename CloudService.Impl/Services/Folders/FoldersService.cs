using CloudService.Configurations;
using CloudService.DAL;
using CloudService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Folders
{
    public class FoldersService : BaseService, IFoldersService
    {
        private readonly PortalConfiguration configuration;

        public FoldersService(
            ApplicationDbContext dbContext,
            IOptions<PortalConfiguration> options
            )
            : base(dbContext)
        {
            configuration = options.Value;
        }

        public async Task Create(string name, string parentId)
        {
            var parent = await Get(parentId);

            var srvPath = configuration.Uploading.FileServerPath;
            var fullPath = Path.Combine(srvPath, $"{parent.Path}\\{name}");

            Directory.CreateDirectory(fullPath);

            var model = new FolderInfo
            {
                Id = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                FolderId = parentId,
                Name = name,
                Size = 0,
                StorageId = parent.StorageId,
                Path = $"{parent.Path}\\{name}"
            };

            DbContext.FolderInfos.Add(model);

            await DbContext.SaveChangesAsync();
        }

        public async Task CreateRoot(string storageId)
        {
            var srvPath = configuration.Uploading.FileServerPath;
            var fullPath = Path.Combine(srvPath, $"{storageId}\\root");

            Directory.CreateDirectory(fullPath);

            var model = new FolderInfo
            {
                Id = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                Name = "root",
                Size = 0,
                StorageId = storageId,
                Path = $"{storageId}\\root"
            };

            DbContext.FolderInfos.Add(model);

            await DbContext.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var model = await DbContext.FolderInfos
                .Include(e => e.ChildFiles)
                .Include(e => e.ChildFolders)
                .FirstOrDefaultAsync(e => e.Id == id);

            var srvPath = configuration.Uploading.FileServerPath;
            var fullPath = Path.Combine(srvPath, $"{model.Path}");

            Directory.Delete(fullPath, true);

            await DeleteChildFolders(model);
            await DeleteChildFiles(model);

            DbContext.FolderInfos.Remove(model);

            await DbContext.SaveChangesAsync();
        }

        public async Task<FolderInfo> Get(string id)
        {
            var model = await DbContext.FolderInfos
                .AsNoTracking()
                .Include(e => e.Storage)
                .FirstOrDefaultAsync(e => e.Id == id);

            return model;
        }

        public async Task<FolderInfo> GetRoot(string storageId)
        {
            var model = await DbContext.FolderInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.StorageId == storageId && e.Name == "root");

            return model;
        }

        public async Task<IEnumerable<FolderInfo>> GetAll(string parentId)
        {
            var folders = await DbContext.FolderInfos
                .AsNoTracking()
                .Where(e => e.FolderId == parentId)
                .ToListAsync();

            return folders;
        }

        public async Task Update(string id, string name)
        {
            var model = await DbContext.FolderInfos
                .Include(e => e.Folder)
                .Include(e => e.ChildFolders)
                .Include(e => e.ChildFiles)
                .FirstOrDefaultAsync(e => e.Id == id);

            var srvPath = configuration.Uploading.FileServerPath;
            var fullPath = Path.Combine(srvPath, $"{model.Path}");

            Directory.Move(fullPath, Path.Combine(srvPath, $"{model.Folder.Path}\\{name}"));

            model.Name = name;
            model.Path = $"{model.Folder.Path}\\{name}";
            model.LastModifiedOn = DateTime.Now;

            DbContext.FolderInfos.Update(model);

            await DbContext.SaveChangesAsync();

            await UpdateChildFoldersPath(model);
            await UpdateChildFilesPath(model);
        }

        public async Task UpdateSize(string id, long bytes)
        {
            var model = await DbContext.FolderInfos
                .Include(e => e.Folder)
                .FirstOrDefaultAsync(e => e.Id == id);

            model.Size += bytes;
            model.LastModifiedOn = DateTime.Now;

            DbContext.FolderInfos.Update(model);

            await DbContext.SaveChangesAsync();

            if (model.Folder != null)
            {
                await UpdateParentSize(model.FolderId, bytes);
            }
            else
            {
                var storage = await DbContext.Storages
                    .FirstOrDefaultAsync(e => e.Id == model.StorageId);

                storage.UsedCapacity += bytes;

                DbContext.Storages.Update(storage);

                await DbContext.SaveChangesAsync();
            }
        }

        private async Task UpdateChildFoldersPath(FolderInfo folder)
        {
            foreach(var childFolder in folder.ChildFolders)
            {
                var currentFolder = await DbContext.FolderInfos
                    .Include(e => e.Folder)
                    .Include(e => e.ChildFolders)
                    .Include(e => e.ChildFiles)
                    .FirstOrDefaultAsync(e => e.Id == childFolder.Id);

                currentFolder.Path = $"{folder.Path}\\{currentFolder.Name}";
                currentFolder.LastModifiedOn = DateTime.Now;

                DbContext.FolderInfos.Update(currentFolder);

                await DbContext.SaveChangesAsync();

                await UpdateChildFoldersPath(currentFolder);
                await UpdateChildFilesPath(currentFolder);
            }
        }

        private async Task UpdateChildFilesPath(FolderInfo folder)
        {
            foreach(var childFile in folder.ChildFiles)
            {
                var currentFile = await DbContext.FileInfos
                    .Include(e => e.Extension)
                    .FirstOrDefaultAsync(e => e.Id == childFile.Id);

                currentFile.Path = $"{folder.Path}\\{currentFile.Name}{currentFile.Extension.Format}";
                currentFile.LastModifiedOn = DateTime.Now;

                DbContext.FileInfos.Update(currentFile);

                await DbContext.SaveChangesAsync();
            }
        }

        private async Task DeleteChildFolders(FolderInfo folder)
        {
            var childIds = folder.ChildFolders
                .Select(e => e.Id)
                .ToArray();

            foreach(var childId in childIds)
            {
                var currentFolder = await DbContext.FolderInfos
                    .Include(e => e.ChildFolders)
                    .Include(e => e.ChildFiles)
                    .FirstOrDefaultAsync(e => e.Id == childId);

                await DeleteChildFolders(currentFolder);
                await DeleteChildFiles(currentFolder);

                DbContext.FolderInfos.Remove(currentFolder);

                await DbContext.SaveChangesAsync();
            }
        }

        private async Task DeleteChildFiles(FolderInfo folder)
        {
            var childIds = folder.ChildFiles
                .Select(e => e.Id)
                .ToArray();

            foreach (var childId in childIds)
            {
                var file = await DbContext.FileInfos
                    .FirstOrDefaultAsync(e => e.Id == childId);

                DbContext.FileInfos.Remove(file);

                await DbContext.SaveChangesAsync();

                await UpdateSize(file.FolderId, -file.Size);
            }
        }

        private async Task UpdateParentSize(string parentId, long bytes)
        {
            var folder = await DbContext.FolderInfos
                .Include(e => e.Folder)
                .FirstOrDefaultAsync(e => e.Id == parentId);

            folder.Size += bytes;

            DbContext.FolderInfos.Update(folder);

            await DbContext.SaveChangesAsync();

            if (folder.Name == "root")
            {
                var storage = await DbContext.Storages
                    .FirstOrDefaultAsync(e => e.Id == folder.StorageId);

                storage.UsedCapacity += bytes;

                DbContext.Storages.Update(storage);

                await DbContext.SaveChangesAsync();
            }
            else
            {
                await UpdateParentSize(folder.FolderId, bytes);
            }
        }
    }
}
