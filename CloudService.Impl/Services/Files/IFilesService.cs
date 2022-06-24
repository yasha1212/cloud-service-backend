using CloudService.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Files
{
    public interface IFilesService
    {
        Task Create(string parentId, IFormFile file);

        Task Update(string id, string name);

        Task<FileInfo> Get(string id);

        Task<IEnumerable<FileInfo>> GetAll(string parentId);

        Task<IEnumerable<FileInfo>> GetAllPublished(string storageId);

        Task Delete(string id);
    }
}
