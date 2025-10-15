namespace SoundTrack.Server.Models
{
    public class ArtistProfile : Profile
    {
        public List<AlbumProfile> Albums { get; set; }
        public List<SongProfile> Songs { get; set; }

        public List<SongProfile> getPopularSongs()
        {
            if (Songs == null || Songs.Count == 0)
            {
                return new List<SongProfile>();
            }
            return Songs.OrderByDescending(s => s.score).Take(5).ToList();
        }

        public List<AlbumProfile> getPopularAlbums()
        {
            if (Albums == null || Albums.Count == 0)
            {
                return new List<AlbumProfile>();
            }
            return Albums.OrderByDescending(a => a.score).Take(5).ToList();
        }
    }
}
