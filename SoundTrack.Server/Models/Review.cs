namespace SoundTrack.Server.Models
{
    public class Review : Comment
    {
        public Score score { get; set; }

        public string? SongProfileId { get; set; }
        public string? ArtistProfileId { get; set; }
        public string? AlbumProfileId { get; set; }
    }
}
