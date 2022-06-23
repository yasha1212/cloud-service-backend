using CloudService.DAL;
using CloudService.Entities;
using CloudService.Impl.Services.Files;
using CloudService.Impl.Services.Folders;
using CloudService.Impl.Services.FrameService.Models;
using CloudService.Impl.Services.Storage;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.FrameService
{
    public class FrameService : BaseService, IFrameService
    {
        private readonly IStorageService storageService;
        private readonly IFoldersService foldersService;
        private readonly IFilesService filesService;
        private readonly UserManager<ApplicationUser> userManager;

        public FrameService(
            ApplicationDbContext dbContext,
            IFoldersService foldersService,
            IFilesService filesService,
            IStorageService storageService,
            UserManager<ApplicationUser> userManager
            )
        : base(dbContext)
        {
            this.storageService = storageService;
            this.foldersService = foldersService;
            this.filesService = filesService;
            this.userManager = userManager;
        }

        public async Task<StorageDto> GetFrame(string folderId, string userName)
        {
            var frameId = folderId;
            FolderInfo root;

            var user = await userManager.FindByNameAsync(userName);

            var storageResult = await storageService.Get(user.Id);

            if (frameId == null)
            {
                root = await foldersService.GetRoot(storageResult.Id);
                frameId = root.Id;
            }

            var foldersResult = await foldersService.GetAll(frameId);
            var filesResult = await filesService.GetAll(frameId);

            return ToViewModel(storageResult, filesResult, foldersResult, frameId);
        }

        public async Task<StorageDto> GetLinksFrame(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);

            var storageResult = await storageService.Get(user.Id);

            var filesResult = await filesService.GetAllPublished(storageResult.Id);

            return ToViewModel(storageResult, filesResult, new List<FolderInfo>(), null);
        }

        private StorageDto ToViewModel(Entities.Storage model, IEnumerable<FileInfo> files, IEnumerable<FolderInfo> folders, string frameId)
        {
            var capacity = CalculateCapacity(model.Capacity);
            var usedCapacity = CalculateCapacity(model.UsedCapacity);
            var percentage = Math.Round(Convert.ToDouble(model.UsedCapacity) / Convert.ToDouble(model.Capacity), 2) * 100;

            var foldersViewModel = folders
                .Select(e =>
                {
                    var size = CalculateCapacity(e.Size);

                    return new FolderDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Size = size.Item1,
                        SizeType = size.Item2
                    };
                })
                .ToList();

            var filesViewModel = files
                .Select(e =>
                {
                    var size = CalculateCapacity(e.Size);

                    return new FileDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Type = e.Extension.Format,
                        IsOpened = !e.IsProtected,
                        Size = size.Item1,
                        SizeType = size.Item2
                    };
                })
                .ToList();

            var result = new StorageDto
            {
                Id = model.Id,
                Name = model.Name,
                Capacity = capacity.Item1,
                CapacityType = capacity.Item2,
                UsedCapacity = usedCapacity.Item1,
                UsedCapacityType = usedCapacity.Item2,
                UsedCapacityPercentage = percentage,
                CurrentFolderId = frameId,
                Folders = foldersViewModel,
                Files = filesViewModel
            };

            return result;
        }

        private (double, string) CalculateCapacity(long bytes)
        {
            var counter = 0;
            double value = Convert.ToDouble(bytes);

            while (value >= 1000)
            {
                value /= 1000;
                counter++;
            }

            value = Math.Round(value, 1);

            string type = "";

            switch (counter)
            {
                case 0:
                    type = "Б";
                    break;
                case 1:
                    type = "КБ";
                    break;
                case 2:
                    type = "МБ";
                    break;
            }

            return (value, type);
        }
    }
}
