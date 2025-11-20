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

			var redirectUrl = "https://127.0.0.1:49825/";

			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

			//Redirige a spotify
			return Challenge(properties, SpotifyAuthenticationDefaults.AuthenticationScheme);
		}

		[HttpGet("callback")]
		public IActionResult Callback()
		{
			// Después del login, redirigir al frontend
			return Redirect("https://localhost:49825/");
		}
	}
}