using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using AspNet.Security.OAuth.Spotify;

namespace SoundTrack.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")] // Crea la ruta base /api/AuthSpotify
	public class AuthSpotifyController : ControllerBase
	{
		[HttpGet("login")] // Completa la ruta /api/AuthSpotify/login
		public IActionResult Login()
		{
			
			var redirectUrl = "https://localhost:49825/";

			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

			//Redirige a spotify
			return Challenge(properties, SpotifyAuthenticationDefaults.AuthenticationScheme);
		}
	}
}