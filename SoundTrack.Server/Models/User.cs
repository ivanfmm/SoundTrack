using Microsoft.AspNetCore.Identity;
using SoundTrack.Server.Data;

namespace SoundTrack.Server.Models
{
    public class User : IdentityUser
    {
     
        public string? Bio { get; set; }
        public string? SpotifyAccessToken { get; set; }
        public string? SpotifyRefreshToken { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime CreateDate { get; set; }

        public List<UserFollow> Followers { get; set; }
        public List<UserFollow> Following { get; set; }
        public List<Review> Reviews { get; set; }

        public string? FavoriteArtistIds { get; set; } 
        public string? FavoriteAlbumIds { get; set; }  
        public string? FavoriteSongIds { get; set; }
        //Trending Tracks usando los id separados por ,
        public List<SongProfile> TrendingTracks { get; set; }
        public List<ArtistProfile> TrendingArtists { get; set; }
        public List<AlbumProfile> TrendingAlbums { get; set; }

        public List<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
        public List<ArtistFollow> FollowedArtists { get; set; } = new List<ArtistFollow>();

        public void FollowUser(UserFollow userToFollow)
        {
            if (Following == null)
            {
                Following = new List<UserFollow>();
            }
            if (!Following.Contains(userToFollow))
            {
                Following.Add(userToFollow);
            }
        }

        public void UnfollowUser(UserFollow userToUnfollow)
        {
            if (Following != null && Following.Contains(userToUnfollow))
            {
                Following.Remove(userToUnfollow);
            }
        }

        // Métodos helper actualizados
        public void FollowUser(User userToFollow, SoundTrackContext context)
        {
            if (userToFollow == null || userToFollow.Id == this.Id)
                return;

            var existingFollow = context.UserFollows
                .FirstOrDefault(uf => uf.FollowerId == this.Id && uf.FollowingId == userToFollow.Id);

            if (existingFollow == null)
            {
                var newFollow = new UserFollow
                {
                    FollowerId = this.Id,
                    FollowingId = userToFollow.Id,
                    FollowDate = DateTime.UtcNow
                };
                context.UserFollows.Add(newFollow);
            }
        }

        public void UnfollowUser(User userToUnfollow, SoundTrackContext context)
        {
            var follow = context.UserFollows
                .FirstOrDefault(uf => uf.FollowerId == this.Id && uf.FollowingId == userToUnfollow.Id);

            if (follow != null)
            {
                context.UserFollows.Remove(follow);
            }
        }
    }
}
