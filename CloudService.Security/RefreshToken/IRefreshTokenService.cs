using System.Threading.Tasks;

namespace CloudService.Security.RefreshToken
{
    public interface IRefreshTokenService
    {
        Entities.RefreshToken Find(string refreshToken);

        Task AddAsync(Entities.RefreshToken token);

        Task UpdateAsync(Entities.RefreshToken token);

        Task RevokeAsync(string refreshToken);
    }
}
