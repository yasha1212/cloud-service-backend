using System.Collections.Generic;

namespace CloudService.Impl.Services.FrameService.Models
{
    public class StorageDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public double Capacity { get; set; }

        public string CapacityType { get; set; }

        public double UsedCapacity { get; set; }

        public string UsedCapacityType { get; set; }

        public double UsedCapacityPercentage { get; set; }

        public string CurrentFolderId { get; set; }

        public List<FolderDto> Folders { get; set; }

        public List<FileDto> Files { get; set; }

        public StorageDto()
        {
            Folders = new List<FolderDto>();
            Files = new List<FileDto>();
        }
    }
}
