using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.Storage
{
    public interface IStorageService
    {
        Task<Entities.Storage> Get(string userId);

        Task<Entities.Storage> Create(string name, string userId);

        Task<Entities.Storage> Update(string name, string userId);
    }
}
