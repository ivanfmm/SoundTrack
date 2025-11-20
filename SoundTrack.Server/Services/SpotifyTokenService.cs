using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<IdentityUser> _userManager;

		public SpotifyTokenService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			UserManager<IdentityUser> userManager)
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

			// Obtener token de AspNetUserTokens
			var accessToken = await _userManager.GetAuthenticationTokenAsync(
				user,
				"Spotify",
				"access_token"
			);

			if (string.IsNullOrEmpty(accessToken))
			{
				throw new Exception("Usuario no autenticado con Spotify");
			}

			return accessToken;
		}

		public async Task<string> RefreshUserTokenAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				throw new Exception("Usuario no encontrado");
			}

			// Obtener refresh token de AspNetUserTokens
			var refreshToken = await _userManager.GetAuthenticationTokenAsync(
				user,
				"Spotify",
				"refresh_token"
			);

			if (string.IsNullOrEmpty(refreshToken))
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
				{ "refresh_token", refreshToken }
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
				throw new Exception("Respuesta de Spotify invalida");
			}

			// Actualizar tokens en AspNetUserTokens
			await _userManager.SetAuthenticationTokenAsync(
				user,
				"Spotify",
				"access_token",
				tokenResponse.AccessToken
			);

			if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
			{
				await _userManager.SetAuthenticationTokenAsync(
					user,
					"Spotify",
					"refresh_token",
					tokenResponse.RefreshToken
				);
			}

			Console.WriteLine($"Token refrescado para {email}");
			return tokenResponse.AccessToken;
		}
	}

	public class SpotifyTokenRefreshResponse
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("refresh_token")]
		public string? RefreshToken { get; set; }

		[JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonPropertyName("scope")]
		public string Scope { get; set; }
	}
}