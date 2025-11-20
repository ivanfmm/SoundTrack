using Microsoft.AspNetCore.Identity;
using SoundTrack.Server.Models; // 👈 AGREGADO
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundTrack.Server.Services
{
	public interface ISpotifyTokenService
	{
		Task<string> GetUserAccessTokenAsync(string email);
		Task<string> RefreshUserTokenAsync(string email);
	}

	public class SpotifyTokenService : ISpotifyTokenService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly UserManager<User> _userManager; // 👈 CAMBIADO: User en lugar de IdentityUser

		public SpotifyTokenService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			UserManager<User> userManager) // 👈 CAMBIADO
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_userManager = userManager;
		}

		public async Task<string> GetUserAccessTokenAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				throw new Exception("Usuario no encontrado");
			}

			// 👇 Ahora obtenemos el token directamente del modelo User
			if (string.IsNullOrEmpty(user.SpotifyAccessToken))
			{
				throw new Exception("Usuario no autenticado con Spotify");
			}

			return user.SpotifyAccessToken;
		}

		public async Task<string> RefreshUserTokenAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				throw new Exception("Usuario no encontrado");
			}

			// 👇 Obtenemos el refresh token del modelo User
			if (string.IsNullOrEmpty(user.SpotifyRefreshToken))
			{
				throw new Exception("Usuario sin refresh token");
			}

			var clientId = _configuration["Spotify:ClientId"];
			var clientSecret = _configuration["Spotify:ClientSecret"];

			var client = _httpClientFactory.CreateClient();
			var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");

			var auth = Convert.ToBase64String(
				Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")
			);
			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);

			request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "grant_type", "refresh_token" },
				{ "refresh_token", user.SpotifyRefreshToken }
			});

			var response = await client.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error al refrescar token: {response.StatusCode} - {error}");
			}

			var responseString = await response.Content.ReadAsStringAsync();
			var tokenResponse = JsonSerializer.Deserialize<SpotifyTokenRefreshResponse>(
				responseString,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
			);

			if (tokenResponse?.AccessToken == null)
			{
				throw new Exception("Respuesta de Spotify inválida");
			}

			// 👇 Actualizamos los tokens en el modelo User
			user.SpotifyAccessToken = tokenResponse.AccessToken;

			if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
			{
				user.SpotifyRefreshToken = tokenResponse.RefreshToken;
			}

			// 👇 Guardamos los cambios
			await _userManager.UpdateAsync(user);

			Console.WriteLine($"✅ Token refrescado para {email}");
			return tokenResponse.AccessToken;
		}
	}

	public class SpotifyTokenRefreshResponse
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; } = string.Empty;

		[JsonPropertyName("refresh_token")]
		public string? RefreshToken { get; set; }

		[JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonPropertyName("scope")]
		public string? Scope { get; set; }
	}
}