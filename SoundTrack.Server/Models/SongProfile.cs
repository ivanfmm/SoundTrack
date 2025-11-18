namespace SoundTrack.Server.Models
{
    public class SongProfile : Profile
    {
        public string AlbumId { get; set; }

        public AlbumProfile Album { get; set; }

        public List<ArtistProfile> Artists { get; set; }
    }
}
