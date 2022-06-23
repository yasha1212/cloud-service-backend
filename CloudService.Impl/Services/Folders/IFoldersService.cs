using CloudService.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Folders
{
    public interface IFoldersService
    {
        Task Create(string name, string parentId);

        Task CreateRoot(string storageId);

        Task Update(string id, string name);

        Task UpdateSize(string id, long bytes);

        Task<FolderInfo> Get(string id);

        Task<FolderInfo> GetRoot(string storageId);

        Task<IEnumerable<FolderInfo>> GetAll(string parentId);

        Task Delete(string id);
    }
}
