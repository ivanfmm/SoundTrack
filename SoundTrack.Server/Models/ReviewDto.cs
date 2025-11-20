namespace SoundTrack.Server.Models
{
    public class ReviewDto
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Score { get; set; }
        public string? SongProfileId { get; set; }
        public string? ArtistProfileId { get; set; }
        public string? AlbumProfileId { get; set; }
    }
}