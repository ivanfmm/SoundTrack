namespace SoundTrack.Server.Models
{
    public class AlbumProfile : Profile
    {
        public List<ArtistProfile> Artist { get; set; }
        public List<SongProfile> Tracks { get; set; }

    }
}
