using SoundTrack.Server.Models;

namespace SoundTrack.Server.Services
{
    public interface ISoundTrackRepository
    {
        //Users
        public Task<List<User>> GetAllUsers();
        public Task<User?> GetUserById(string id);
        public Task<User?> GetUserByUsername(string username);
        public void AddUser(User user);
        public void UpdateUser(User user);
        public void DeleteUser(User user);
        //Profile 
        Task<double?> GetAverageScore(string profileId, string profileType);
        Task<int> GetReviewCountByProfile(string profileId, string profileType);

        //ArtistProfile
        public Task<List<ArtistProfile>> GetAllArtistProfile();
        public Task<ArtistProfile?> GetArtistProfileById(string id);
        public Task<ArtistProfile?> GetArtistProfileByName(string name);
        public void addArtistProfile(ArtistProfile artistProfile);
        public void updateArtistProfile(ArtistProfile artistProfile);
        public void deleteArtistProfile(ArtistProfile artistProfile);

        //ArtistFollow
        public Task<ArtistFollow?> FollowArtist(string userId, string artistId);
        public Task<bool> UnfollowArtist(string userId, string artistId);
        public Task<bool> IsFollowingArtist(string userId, string artistId);
        public Task<List<ArtistProfile>> GetUserFollowedArtists(string userId);
        public Task<int> GetArtistFollowersCount(string artistId);
        public Task<List<User>> GetArtistFollowers(string artistId);

        //AlbumProfile
        public Task<List<AlbumProfile>> GetAllAlbumProfile();
        public Task<AlbumProfile?> GetAlbumProfileById(string id);
        public Task<AlbumProfile?> GetAlbumProfileByName(string name);
        public void addAlbumProfile(AlbumProfile albumProfile);
        public void updateAlbumProfile(AlbumProfile albumProfile);
        public void deleteAlbumProfile(AlbumProfile albumProfile);


        //SongProfile
        public Task<List<SongProfile>> GetAllSongProfile();
        public Task<SongProfile?> GetSongProfileById(string id);
        public Task<SongProfile?> GetSongProfileByName(string name);
        public void addSongProfile(SongProfile songProfile);
        public void updateSongProfile(SongProfile songProfile);
        public void deleteSongProfile(SongProfile songProfile);

        //Reviews
        public Task<List<Review>> GetAllReviews();
        public Task<Review?> GetReviewById(int id);
        public Task addReview(Review review);

        public void updateReview(Review review);
        public void deleteReview(Review review);
        public Task<(Review? review, LikeType newStatus)> ToggleLike(int reviewId, string userId);
        public Task<(Review? review, LikeType newStatus)> ToggleDislike(int reviewId, string userId);
        public Task<LikeType> GetUserLikeStatus(int reviewId, string userId);

        //ReviewComments
        public Task<List<ReviewComment>> GetAllReviewComments();
        public Task<ReviewComment?> GetReviewCommentById(int id);
        public void addReviewComment(ReviewComment reviewComment);
        public void updateReviewComment(ReviewComment reviewComment);
        public void deleteReviewComment(ReviewComment reviewComment);

        //Falta logica para los Likes y Dislikes que se guarden en memoria (pensaba hacer un tipo de array que se guarde, para aumentar los datos en la base de datos todo de una)\
        //Se pueden agregar funciones para eliminar
    }
}
