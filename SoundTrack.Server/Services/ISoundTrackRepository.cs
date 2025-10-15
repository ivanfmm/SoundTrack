using SoundTrack.Server.Models;

namespace SoundTrack.Server.Services
{
    public interface ISoundTrackRepository
    {
        //Users
        public List<User> GetAllUsers();
        public User? GetUserById(int id);
        public User? GetUserByUsername(string username);
        public void AddUser(User user);
        public void UpdateUser(User user);

        //ArtistProfile
        public List<ArtistProfile> GetAllArtistProfile();
        public ArtistProfile? GetArtistProfileById(int id);
        public ArtistProfile? GetArtistProfileByName(string username);
        public void addArtistProfile(ArtistProfile artistProfile);
        public void updateArtistProfile(ArtistProfile artistProfile);

        //AlbumProfile
        public List<AlbumProfile> GetAllAlbumProfile();
        public AlbumProfile? GetAlbumProfileById(int id);
        public AlbumProfile? GetAlbumProfileByName(string username);
        public void addAlbumProfile(AlbumProfile albumProfile);
        public void updateAlbumProfile(AlbumProfile albumProfile);


        //SongProfile
        public List<SongProfile> GetAllSongProfile();
        public SongProfile? GetSongProfileById(int id);
        public SongProfile? GetSongProfileByName(string username);
        public void addSongProfile(SongProfile songProfile);
        public void updateSongProfile(SongProfile songProfile);

        //Reviews
        public List<Review> GetAllReviews();
        public Review? GetReviewById(int id);
        public void addReview(Review review);
        public void updateReview(Review review);

        //ReviewComments
        public List<ReviewComment> GetAllReviewComments();
        public ReviewComment? GetReviewCommentById(int id);
        public void addReviewComment(ReviewComment reviewComment);
        public void updateReviewComment(ReviewComment reviewComment);

        //Falta logica para los Likes y Dislikes que se guarden en memoria (pensaba hacer un tipo de array que se guarde, para aumentar los datos en la base de datos todo de una)\
        //Se pueden agregar funciones para eliminar
    }
}
