using CloudService.Entities;
using CloudService.Security.RefreshToken;
using CloudService.Security.Token;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Security.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

        public AuthService(
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<IdentityResult> Register(ApplicationUser user, string password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "DefaultUser");
            }

            return result;
        }

        public async Task<bool> DoesUserExist(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user != null;
        }

        public async Task<TokenInfo> Login(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                throw new Exception("Incorrect login.");
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                throw new Exception("Incorrect password.");
            }

            return await _tokenService.CreateTokenInfo(user);
        }

        public async Task<TokenInfo> VerifyAndReturnNewToken(string refreshToken)
        {
            try
            {
                 var storedRefreshToken = _refreshTokenService.Find(refreshToken);

                if (storedRefreshToken == null)
                {
                    throw new Exception("Refresh token doesn't exist");
                }

                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    throw new Exception("Token has expired, user has to relogin");
                }

                if (storedRefreshToken.IsRevoked)
                {
                    throw new Exception("Token has been revoked");
                }

                var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);

                var claimsPrincipal = await _claimsFactory.CreateAsync(user);

                var token = _tokenService.CreateAccessToken(claimsPrincipal);

                var tokenInfo = new TokenInfo()
                {
                    RefreshToken = refreshToken,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                };

                return tokenInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
