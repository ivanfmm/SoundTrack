using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using AspNet.Security.OAuth.Spotify;

namespace SoundTrack.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")] // Crea la ruta base /api/AuthSpotify
	public class AuthSpotifyController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public AuthSpotifyController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// A donde regresar despues del login: "/" en Vercel (mismo dominio), el puerto de Vite en local
		private string FrontendUrl => _configuration["FrontendUrl"] ?? "/";

		[HttpGet("login")] // Completa la ruta /api/AuthSpotify/login
		public IActionResult Login()
		{

			var redirectUrl = FrontendUrl;

			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

			//Redirige a spotify
			return Challenge(properties, SpotifyAuthenticationDefaults.AuthenticationScheme);
		}

		[HttpGet("callback")]
		public IActionResult Callback()
		{
			// Despues del login redirige al frontend
			return Redirect(FrontendUrl);
		}
	}
}