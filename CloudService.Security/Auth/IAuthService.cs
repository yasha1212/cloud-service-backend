using CloudService.Entities;
using CloudService.Security.Token;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CloudService.Security.Auth
{
    public interface IAuthService
    {
        Task<TokenInfo> Login(string userName, string password);

        Task<IdentityResult> Register(ApplicationUser user, string password);

        Task<bool> DoesUserExist(string userName);

        Task<TokenInfo> VerifyAndReturnNewToken(string refreshToken);
    }
}
