using CloudService.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CloudService.Security.Claims
{
	public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
	{
		public ClaimsPrincipalFactory(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
		{
		}

		public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
		{
			ClaimsPrincipal principal = await base.CreateAsync(user);

			if (principal.Identity is ClaimsIdentity identity)
			{
				var claims = new List<Claim>();

				claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

				identity.AddClaims(claims);
			}

			return principal;
		}
	}
}
