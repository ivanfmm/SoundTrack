using Microsoft.EntityFrameworkCore;
using SoundTrack.Server.Data;
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
		private readonly SoundTrackContext _context;

		public SpotifyTokenService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			SoundTrackContext context)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_context = context;
		}
		public async Task<string> GetUserAccessTokenAsync(string email)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			if (user == null || string.IsNullOrEmpty(user.SpotifyAccessToken))
			{
				throw new Exception("Usuario no autenticado con Spotify");
			}

			//Para verificar si el token es valido
			return user.SpotifyAccessToken;
		}

		public async Task<string> RefreshUserTokenAsync(string email)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			if (user == null || string.IsNullOrEmpty(user.SpotifyRefreshToken))
			{
				throw new Exception("Usuario sin refresh token");
			}

			var clientId = _configuration["Spotify:ClientId"];
			var clientSecret = _configuration["Spotify:ClientSecret"];

			var client = _httpClientFactory.CreateClient();
			var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");

			// Autenticación Basic
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
				throw new Exception("Respuesta de Spotify invalida");
			}

			// Actualizar en DB
			user.SpotifyAccessToken = tokenResponse.AccessToken;

			// Spotify puede devolver nuevo refresh token (opcional)
			if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
			{
				user.SpotifyRefreshToken = tokenResponse.RefreshToken;
			}

			await _context.SaveChangesAsync();

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