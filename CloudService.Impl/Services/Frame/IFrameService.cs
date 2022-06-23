using CloudService.Impl.Services.FrameService.Models;
using System.Threading.Tasks;

namespace CloudService.Impl.Services.FrameService
{
    public interface IFrameService
    {
        Task<StorageDto> GetFrame(string folderId, string userName);

        Task<StorageDto> GetLinksFrame(string userName);
    }
}
