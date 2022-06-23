using CloudService.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Files
{
    public interface IFilesService
    {
        Task Create(string name, string parentId);

        Task Update(string id, string name);

        Task<FileInfo> Get(string id);

        Task<IEnumerable<FileInfo>> GetAll(string parentId);

        Task<IEnumerable<FileInfo>> GetAllPublished(string storageId);

        Task Delete(string id);
    }
}
