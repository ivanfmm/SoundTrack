using Microsoft.AspNetCore.Identity;

namespace SoundTrack.Server.Models
{
	public class User : IdentityUser
	{
		// ===== PROPIEDADES PERSONALIZADAS =====

		// Email ya existe en IdentityUser, pero lo redefinimos para evitar warnings
		public new string? Email
		{
			get => base.Email;
			set => base.Email = value;
		}

		public string? Bio { get; set; }
		public string? ProfilePictureUrl { get; set; }
		public DateTime? BirthDay { get; set; }
		public DateTime CreateDate { get; set; } = DateTime.UtcNow;

		// Spotify tokens
		public string? SpotifyAccessToken { get; set; }
		public string? SpotifyRefreshToken { get; set; }

		// Favoritos (CSV o JSON)
		public string? FavoriteArtistIds { get; set; }
		public string? FavoriteAlbumIds { get; set; }
		public string? FavoriteSongIds { get; set; }

		// ===== RELACIONES =====

		// Follows entre usuarios
		public ICollection<UserUser> Followers { get; set; } = new List<UserUser>();
		public ICollection<UserUser> Following { get; set; } = new List<UserUser>();

		// Reviews y likes
		public ICollection<Review> Reviews { get; set; } = new List<Review>();
		public ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();

		// Artistas seguidos
		public ICollection<ArtistFollow> FollowedArtists { get; set; } = new List<ArtistFollow>();

		// Trending items (muchos a muchos, necesitarás configurarlos en DbContext)
		public ICollection<SongProfile> TrendingTracks { get; set; } = new List<SongProfile>();
		public ICollection<ArtistProfile> TrendingArtists { get; set; } = new List<ArtistProfile>();
		public ICollection<AlbumProfile> TrendingAlbums { get; set; } = new List<AlbumProfile>();
	}
}