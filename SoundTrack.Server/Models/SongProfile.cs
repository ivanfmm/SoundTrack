namespace SoundTrack.Server.Models
{
    public class SongProfile : Profile
    {
        public AlbumProfile Album { get; set; }
        public List<ArtistProfile> Artists { get; set; }
    }
}
