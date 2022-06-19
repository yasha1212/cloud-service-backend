using CloudService.Configurations;
using CloudService.Entities;
using CloudService.Security.RefreshToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CloudService.Security.Token
{
    public class TokenService : ITokenService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly PortalConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;

        public TokenService(
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<PortalConfiguration> options,
            IRefreshTokenService refreshTokenService)
        {
            _claimsFactory = claimsFactory;
            _configuration = options.Value;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<TokenInfo> CreateTokenInfo(ApplicationUser user)
        {
            var claimsPrincipal = await _claimsFactory.CreateAsync(user);

            var token = CreateAccessToken(claimsPrincipal);

            var refreshToken = CreateRefreshToken(user);

            await _refreshTokenService.AddAsync(refreshToken);

            return new TokenInfo
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken.Token,
                Expiration = token.ValidTo
            };
        }

        public JwtSecurityToken CreateAccessToken(ClaimsPrincipal claimsPrincipal)
        {
            return new JwtSecurityToken(
                issuer: _configuration.Jwt.Issuer,
                audience: _configuration.Cors.Origins.First(),
                expires: DateTime.Now.AddHours(3),
                claims: claimsPrincipal.Claims,
                signingCredentials: new SigningCredentials(CreateSigningKey(), SecurityAlgorithms.HmacSha256));
        }

        private SymmetricSecurityKey CreateSigningKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.Jwt.Secret));
        }

        private string GenerateRandomStringForRefreshToken()
        {
            const int randomStringLength = 25;

            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, randomStringLength)
                .Select(s => s[random.Next(s.Length)]).ToArray()) + Guid.NewGuid();
        }

        private Entities.RefreshToken CreateRefreshToken(ApplicationUser user)
        {
            return new Entities.RefreshToken
            {
                IsRevoked = false,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(3),
                Token = GenerateRandomStringForRefreshToken()
            };
        }
    }
}
