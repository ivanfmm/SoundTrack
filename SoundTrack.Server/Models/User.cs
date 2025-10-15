namespace SoundTrack.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime BirthDay { get; set; }
        public DateTime CreateDate { get; set; }

        public List<User> Followers { get; set; }
        public List<User> Following { get; set; }
        public List<Review> Reviews { get; set; }
        public List<SongProfile> TrendingTracks { get; set; }
        public List<ArtistProfile> TrendingArtists { get; set; }
        public List<AlbumProfile> TrendingAlbums { get; set; }

        public void FollowUser(User userToFollow)
        {
            if (Following == null)
            {
                Following = new List<User>();
            }
            if (!Following.Contains(userToFollow))
            {
                Following.Add(userToFollow);
            }
        }

        public void UnfollowUser(User userToUnfollow)
        {
            if (Following != null && Following.Contains(userToUnfollow))
            {
                Following.Remove(userToUnfollow);
            }
        }
    }
}
