using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundTrack.Server.Data;
using SoundTrack.Server.Models;
using System.Net.Http;
using System.Text.Json;
//Para poder leer las respuestas de spotify
using System.Text.Json.Serialization;

namespace SoundTrack.Server.Services
{

    public interface ISpotifyProfileService
    {
        Task<SongProfile> EnsureSongProfileExists(string spotifyId);
        Task<ArtistProfile> EnsureArtistProfileExists(string spotifyId);
        Task<AlbumProfile> EnsureAlbumProfileExists(string spotifyId);

		Task<string> GetAccessTokenAsync(); //Para poder pasar access token al front end
		Task<string> GetUserAccessTokenAsync(string email);//Para poder pasar access token del usuario al front end
	}

    public class SpotifyProfileService : ISpotifyProfileService
    {
       
		private readonly SoundTrackContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
		private readonly ISpotifyTokenService _tokenService;


		public SpotifyProfileService(
            SoundTrackContext context,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ISpotifyTokenService tokenService)
        {

			_context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
        }

	    public async Task<string> GetAccessTokenAsync() //Para poder pasar access token al front end.
	    {
	    	return await GetSpotifyToken();
	    }


        public async Task<string> GetUserAccessTokenAsync(string email) //Para poder pasar access token del usuario al front end
	{
        try
        {
            return await _tokenService.GetUserAccessTokenAsync(email);
		}
        catch
        {
            return await _tokenService.RefreshUserTokenAsync(email);
        }
    }

	/// <summary>
	/// Verifica si existe un SongProfile, si no, lo crea con sus dependencias
	/// </summary>
	public async Task<SongProfile> EnsureSongProfileExists(string spotifyId)
        {
            // 1. Verificar si ya existe
            var existingSong = await _context.SongProfiles
                .Include(s => s.Album)
                .Include(s => s.Artists)
                .FirstOrDefaultAsync(s => s.Id == spotifyId);

            if (existingSong != null)
                return existingSong;

            // 2. No existe, obtener de Spotify
            var token = await GetSpotifyToken();
            var spotifyData = await GetSpotifyTrack(spotifyId, token);

            if (spotifyData == null)
                throw new Exception($"No se pudo obtener la canción {spotifyId} de Spotify");

            // 3. Crear artista si no existe
            var artist = await EnsureArtistProfileExists(spotifyData.Artists[0].Id);

            // 4. Crear álbum si no existe
            var album = await EnsureAlbumProfileExists(spotifyData.Album.Id);

            // 5. Conectar artista con álbum si no están conectados
            if (!album.Artist.Any(a => a.Id == artist.Id))
            {
                album.Artist.Add(artist);
                await _context.SaveChangesAsync();
            }

            // 6. Crear canción
            var song = new SongProfile
            {
                Id = spotifyData.Id,
                Name = spotifyData.Name,
                ImageUrl = spotifyData.Album.Images.FirstOrDefault()?.Url ?? "",
                score = Score.ThreeStars, // Score por defecto
                PublicationDate = DateTime.Parse(spotifyData.Album.ReleaseDate).ToUniversalTime(),
                Genres = new List<string>(), // Spotify tracks no tienen géneros directos
                Tags = new List<string>(),
                Description = $"Canción del álbum {spotifyData.Album.Name}",
                AlbumId = album.Id, // ⭐ CRÍTICO
                Album = album,
                Artists = new List<ArtistProfile> { artist }
            };

            _context.SongProfiles.Add(song);
            await _context.SaveChangesAsync();

            return song;
        }

        /// <summary>
        /// Verifica si existe un ArtistProfile, si no, lo crea
        /// </summary>
        public async Task<ArtistProfile> EnsureArtistProfileExists(string spotifyId)
        {
            var existing = await _context.ArtistProfiles
                .FirstOrDefaultAsync(a => a.Id == spotifyId);

            if (existing != null)
                return existing;

            var token = await GetSpotifyToken();
            var spotifyData = await GetSpotifyArtist(spotifyId, token);

            if (spotifyData == null)
                throw new Exception($"No se pudo obtener el artista {spotifyId} de Spotify");

            var artist = new ArtistProfile
            {
                Id = spotifyData.Id,
                Name = spotifyData.Name,
                ImageUrl = spotifyData.Images.FirstOrDefault()?.Url ?? "",
                score = Score.ThreeStars,
                PublicationDate = DateTime.UtcNow,
                Genres = spotifyData.Genres ?? new List<string>(),
                Tags = new List<string>(),
                Description = $"{spotifyData.Name} tiene {spotifyData.Followers.Total:N0} seguidores"
            };

            _context.ArtistProfiles.Add(artist);
            await _context.SaveChangesAsync();

            return artist;
        }

        /// <summary>
        /// Verifica si existe un AlbumProfile, si no, lo crea
        /// </summary>
        public async Task<AlbumProfile> EnsureAlbumProfileExists(string spotifyId)
        {
            var existing = await _context.AlbumProfiles
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.Id == spotifyId);

            if (existing != null)
                return existing;

            var token = await GetSpotifyToken();
            var spotifyData = await GetSpotifyAlbum(spotifyId, token);

            if (spotifyData == null)
                throw new Exception($"No se pudo obtener el álbum {spotifyId} de Spotify");

            var album = new AlbumProfile
            {
                Id = spotifyData.Id,
                Name = spotifyData.Name,
                ImageUrl = spotifyData.Images.FirstOrDefault()?.Url ?? "",
                score = Score.ThreeStars,
                PublicationDate = DateTime.Parse(spotifyData.ReleaseDate).ToUniversalTime(),
                Genres = spotifyData.Genres ?? new List<string>(),
                Tags = new List<string>(),
                Description = $"Álbum con {spotifyData.TotalTracks} canciones",
                Artist = new List<ArtistProfile>() // Se llenará cuando se cree la canción
            };

            _context.AlbumProfiles.Add(album);
            await _context.SaveChangesAsync();

            return album;
        }

        // =============== MÉTODOS PRIVADOS PARA SPOTIFY API ===============

        private async Task<string> GetSpotifyToken() 
        {
            try
            {
                var clientId = _configuration["Spotify:ClientId"];
                var clientSecret = _configuration["Spotify:ClientSecret"];

                // ⭐ VALIDAR que las credenciales existen
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new Exception($"Credenciales de Spotify no configuradas. ClientId: {clientId ?? "NULL"}, ClientSecret: {(string.IsNullOrEmpty(clientSecret) ? "NULL" : "***")}");
                }

                Console.WriteLine($"🔑 Intentando obtener token con ClientId: {clientId}");

                var authValue = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")
                );

                var httpClient = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
                request.Headers.Add("Authorization", $"Basic {authValue}");

                // ⭐ IMPORTANTE: Content-Type debe ser application/x-www-form-urlencoded
                request.Content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                // ⭐ MEJOR MANEJO DE ERRORES
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error obteniendo token de Spotify:");
                    Console.WriteLine($"   Status: {response.StatusCode}");
                    Console.WriteLine($"   Response: {content}");
                    throw new Exception($"Spotify API error: {response.StatusCode} - {content}");
                }

                var data = JsonSerializer.Deserialize<SpotifyTokenResponse>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (data == null || string.IsNullOrEmpty(data.AccessToken))
                {
                    throw new Exception("No se pudo deserializar el token de Spotify");
                }

                Console.WriteLine($"✅ Token obtenido exitosamente");
                return data.AccessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR en GetSpotifyToken: {ex.Message}");
                throw;
            }
        }

        private async Task<SpotifyTrack?> GetSpotifyTrack(string trackId, string token)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.spotify.com/v1/tracks/{trackId}"
            );
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<SpotifyTrack>(
                content,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }

        private async Task<SpotifyArtist?> GetSpotifyArtist(string artistId, string token)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.spotify.com/v1/artists/{artistId}"
            );
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<SpotifyArtist>(
                content,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }

        private async Task<SpotifyAlbum?> GetSpotifyAlbum(string albumId, string token)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.spotify.com/v1/albums/{albumId}"
            );
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<SpotifyAlbum>(
                content,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }
    }

    // =============== CLASES PARA DESERIALIZAR RESPUESTAS DE SPOTIFY ===============

    public class SpotifyTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class SpotifyTrack
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("album")]
        public SpotifyAlbum Album { get; set; }

        [JsonPropertyName("artists")]
        public List<SpotifyArtist> Artists { get; set; }
    }

    public class SpotifyArtist
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("images")]
        public List<SpotifyImage> Images { get; set; }

        [JsonPropertyName("followers")]
        public SpotifyFollowers Followers { get; set; }

        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; }
    }

    public class SpotifyAlbum
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("images")]
        public List<SpotifyImage> Images { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("total_tracks")]
        public int TotalTracks { get; set; }

        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; }
    }

    public class SpotifyImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    public class SpotifyFollowers
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
