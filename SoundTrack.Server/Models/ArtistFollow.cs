namespace SoundTrack.Server.Models
{
    public class ArtistFollow
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public string ArtistProfileId { get; set; }
        public ArtistProfile ArtistProfile { get; set; }

        public DateTime FollowDate { get; set; }
    }
}
