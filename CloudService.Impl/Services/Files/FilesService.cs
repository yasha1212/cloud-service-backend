using CloudService.Configurations;
using CloudService.DAL;
using CloudService.Entities;
using CloudService.Impl.Services.Folders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Files
{
    public class FilesService : BaseService, IFilesService
    {
        private readonly IFoldersService foldersService;
        private readonly PortalConfiguration configuration;

        public FilesService(
            ApplicationDbContext dbContext,
            IFoldersService foldersService,
            IOptions<PortalConfiguration> options
            ) 
            : base(dbContext)
        {
            this.foldersService = foldersService;
            configuration = options.Value;
        }

        public async Task Create(string parentId, IFormFile file)
        {
            var parent = await foldersService.Get(parentId);

            var pathToSave = Path.Combine(configuration.Uploading.FileServerPath, parent.Path);

            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

            var fullPath = Path.Combine(pathToSave, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var nameAndExtension = GetNameAndExtension(fileName);

            var extension = await DbContext.Extensions
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Format == nameAndExtension.Extension);

            var model = new Entities.FileInfo
            {
                Id = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                ExtensionId = extension.Id,
                FolderId = parentId,
                IsProtected = true,
                Name = nameAndExtension.Name,
                Size = file.Length,
                StorageId = parent.StorageId,
                Path = $"{parent.Path}\\{nameAndExtension.Name}{extension.Format}"
            };

            DbContext.FileInfos.Add(model);

            await DbContext.SaveChangesAsync();

            await foldersService.UpdateSize(parentId, model.Size);
        }

        public async Task Delete(string id)
        {
            var model = await DbContext.FileInfos
                .FirstOrDefaultAsync(e => e.Id == id);

            var srvPath = configuration.Uploading.FileServerPath;
            var fullPath = Path.Combine(srvPath, $"{model.Path}");

            File.Delete(fullPath);

            DbContext.FileInfos.Remove(model);

            await DbContext.SaveChangesAsync();

            await foldersService.UpdateSize(model.FolderId, -model.Size);
        }

        public async Task<Entities.FileInfo> Get(string id)
        {
            var model = await DbContext.FileInfos
                .AsNoTracking()
                .Include(e => e.Extension)
                .Include(e => e.Storage)
                .FirstOrDefaultAsync(e => e.Id == id);

            return model;
        }

        public async Task<(MemoryStream File, string MimeType, string Name)> GetForDownload(string id)
        {
            var model = await Get(id);
            var memory = new MemoryStream();
            var fullPath = Path.Combine(configuration.Uploading.FileServerPath, model.Path);

            await using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            var provider = new FileExtensionContentTypeProvider();

            string contentType;

            if (!provider.TryGetContentType(fullPath, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return (memory, contentType, $"{model.Name}{model.Extension.Format}");
        }

        public async Task<IEnumerable<Entities.FileInfo>> GetAll(string parentId)
        {
            var files = await DbContext.FileInfos
                .Include(e => e.Extension)
                .AsNoTracking()
                .Where(e => e.FolderId == parentId)
                .ToListAsync();

            return files;
        }

        public async Task<IEnumerable<Entities.FileInfo>> GetAllPublished(string storageId)
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

            var srvPath = configuration.Uploading.FileServerPath;
            var fullPath = Path.Combine(srvPath, $"{model.Path}");

            File.Move(fullPath, Path.Combine(srvPath, $"{model.Folder.Path}\\{name}{model.Extension.Format}"));

            model.Name = name;
            model.Path = $"{model.Folder.Path}\\{name}{model.Extension.Format}";
            model.LastModifiedOn = DateTime.Now;

            DbContext.FileInfos.Update(model);

            await DbContext.SaveChangesAsync();
        }

        private (string Name, string Extension) GetNameAndExtension(string fullName)
        {
            var extensionStartIndex = fullName.LastIndexOf('.');

            var extension = fullName.Substring(extensionStartIndex);
            var name = fullName.Substring(0, extensionStartIndex);

            return (Name: name, Extension: extension);
        }
    }
}
