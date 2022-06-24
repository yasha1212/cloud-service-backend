using CloudService.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Files
{
    public interface IFilesService
    {
        Task Create(string parentId, IFormFile file);

        Task Update(string id, string name);

        Task<Entities.FileInfo> Get(string id);

        Task<(MemoryStream File, string MimeType, string Name)> GetForDownload(string id);

        Task<IEnumerable<Entities.FileInfo>> GetAll(string parentId);

        Task<IEnumerable<Entities.FileInfo>> GetAllPublished(string storageId);

        Task Delete(string id);
    }
}
