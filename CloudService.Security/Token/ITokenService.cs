using CloudService.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CloudService.Security.Token
{
    public interface ITokenService
    {
        Task<TokenInfo> CreateTokenInfo(ApplicationUser user);

        JwtSecurityToken CreateAccessToken(ClaimsPrincipal claimsPrincipal);
    }
}
