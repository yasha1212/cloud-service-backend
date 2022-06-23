using CloudService.Impl.Services.Files;
using CloudService.Impl.Services.Folders;
using CloudService.Impl.Services.Sharing;
using CloudService.Impl.Services.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CloudService.Impl
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddImpl(this IServiceCollection services)
        {
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IFoldersService, FoldersService>();
            services.AddScoped<ISharingService, SharingService>();
            services.AddScoped<IFilesService, FilesService>();

            return services;
        }
    }
}
