using CloudService.Entities;
using CloudService.Security.Auth;
using CloudService.Security.RefreshToken;
using CloudService.Web.ViewModels.Auth;
using CloudService.Web.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CloudService.Web.Controllers
{
    [Route("auth")]
    public class AuthController : ApiController
    {
		private readonly IAuthService authService;
		private readonly IRefreshTokenService refreshTokenService;

		public AuthController(
			IAuthService authService,
			IRefreshTokenService refreshTokenService
		)
		{
			this.authService = authService;
			this.refreshTokenService = refreshTokenService;
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel model)
		{
			try
			{
				var token = await authService.Login(model.Email, model.Password);

				var refreshTokenInfo = refreshTokenService.Find(token.RefreshToken);

				Response.Cookies.Append("refreshToken", token.RefreshToken, CreateRefreshTokenCookieOptions(refreshTokenInfo));

				return Ok(new TokenViewModel
				{
					Token = token.Token,
					Expiration = token.Expiration
				});
			}
			catch
			{
				return Unauthorized();
			}
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
		{
			if (await authService.DoesUserExist(model.Email))
			{
				return StatusCode(StatusCodes.Status500InternalServerError, CreateError("User already exists!"));
			}

			if (!model.Password.Equals(model.PasswordRepeat))
            {
				return StatusCode(StatusCodes.Status500InternalServerError, CreateError("Passwords are not the same!"));
			}

			ApplicationUser user = new ApplicationUser()
			{
				Email = model.Email,
				UserName = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName
			};

			var result = await authService.Register(user, model.Password);

			if (!result.Succeeded)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, CreateError("User creation failed! Please check user details and try again."));
			}

			var token = await authService.Login(model.Email, model.Password);

			var refreshTokenInfo = refreshTokenService.Find(token.RefreshToken);

			Response.Cookies.Append("refreshToken", token.RefreshToken, CreateRefreshTokenCookieOptions(refreshTokenInfo));

			return Ok(new TokenViewModel
			{
				Token = token.Token,
				Expiration = token.Expiration
			});
		}

		[HttpPost]
		[Route("refreshToken")]
		public async Task<IActionResult> RefreshToken()
		{
			try
			{
				var refreshToken = Request.Cookies["refreshToken"];

				var res = await authService.VerifyAndReturnNewToken(refreshToken);

				var tokenViewModel = new TokenViewModel
				{
					Token = res.Token,
					Expiration = res.Expiration
				};

				return Ok(tokenViewModel);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("logout")]
		public IActionResult Logout()
		{
			try
			{
				var refreshToken = Request.Cookies["refreshToken"];

				Response.Cookies.Append("refreshToken", "", CreateRefreshTokenCookieOptions(null));

				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		private ResponseViewModel CreateError(string description)
		{
			return new ResponseViewModel { Status = "Error", Message = description };
		}

		private CookieOptions CreateRefreshTokenCookieOptions(RefreshToken refreshTokenInfo)
		{
			var cookieOptions = new CookieOptions();

			cookieOptions.HttpOnly = true;
			cookieOptions.SameSite = SameSiteMode.None;
			cookieOptions.Secure = true;

			if (refreshTokenInfo == null)
			{
				cookieOptions.Expires = DateTimeOffset.UtcNow;
				cookieOptions.MaxAge = TimeSpan.Zero;
			}
			else
			{
				cookieOptions.Expires = refreshTokenInfo.ExpiryDate;
			}

			return cookieOptions;
		}
	}
}
