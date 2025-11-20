using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Services;
using System.Security.Claims;

namespace SoundTrack.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SpotifyController : Controller
	{
		//Inyeccion de dependencia 
		private readonly ISpotifyProfileService _spotifyService;

		public SpotifyController(ISpotifyProfileService spotifyService)
		{
			_spotifyService = spotifyService;
		}

		// GET: api/Spotify/token
		[HttpGet("token")]
		public async Task<IActionResult> GetAccessToken()
		{
			try
			{
				// Para poder sacar el access token
				var token = await _spotifyService.GetAccessTokenAsync();


				return Ok(new { access_token = token });
			}
			catch (Exception ex)
			{

				return StatusCode(500, new { message = ex.Message });
			}
		}

		[HttpGet("user-token")]
		public async Task<IActionResult> GetUserAccessToken()
		{
			try
			{
				var email = User.FindFirst(ClaimTypes.Email)?.Value;

				if (string.IsNullOrEmpty(email))
				{
					return Unauthorized(new { message = "Usuario no autenticado" });
				}
				var token = await _spotifyService.GetUserAccessTokenAsync(email);
				return Ok(new { access_token = token });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}
	}
}
