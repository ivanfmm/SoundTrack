// DTOs/UserProfileDto.cs
namespace SoundTrack.Server.DTOs
{
    public class UserProfileDto
    {
        public string Id { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public UserStatisticsDto Statistics { get; set; } = new();
        public UserFavoritesDto Favorites { get; set; } = new();
    }

    public class UserStatisticsDto
    {
        public int TotalReviews { get; set; }
        public int FollowedArtists { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
    }

    public class UserFavoritesDto
    {
        public List<string> Artists { get; set; } = new();
        public List<string> Albums { get; set; } = new();
        public List<string> Songs { get; set; } = new();
    }
}