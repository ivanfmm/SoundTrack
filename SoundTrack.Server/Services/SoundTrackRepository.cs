using SoundTrack.Server.Models;
using SoundTrack.Server.Data;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;

namespace SoundTrack.Server.Services
{
    public class SoundTrackRepository : ISoundTrackRepository
    {
        private readonly SoundTrackContext _context;

        public SoundTrackRepository(SoundTrackContext context)
        {
            _context = context;
        }

        //Users
        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        //Profile (basicamente el promedio de las estrellas)
        public async Task<double?> GetAverageScore(string profileId, string profileType)
        {
            List<Review> reviews;

            switch (profileType.ToLower())
            {
                case "song":
                    reviews = await _context.Reviews
                        .Where(r => r.SongProfileId == profileId)
                        .ToListAsync();
                    break;
                case "artist":
                    reviews = await _context.Reviews
                        .Where(r => r.ArtistProfileId == profileId)
                        .ToListAsync();
                    break;
                case "album":
                    reviews = await _context.Reviews
                        .Where(r => r.AlbumProfileId == profileId)
                        .ToListAsync();
                    break;
                default:
                    return null;
            }

            if (reviews.Count == 0)
                return null;

            return reviews.Average(r => (int)r.score);
        }

        //contar para hacer el promedio
        public async Task<int> GetReviewCountByProfile(string profileId, string profileType)
        {
            switch (profileType.ToLower())
            {
                case "song":
                    return await _context.Reviews.CountAsync(r => r.SongProfileId == profileId);
                case "artist":
                    return await _context.Reviews.CountAsync(r => r.ArtistProfileId == profileId);
                case "album":
                    return await _context.Reviews.CountAsync(r => r.AlbumProfileId == profileId);
                default:
                    return 0;
            }
        }

        //ArtistProfile
        public async Task<List<ArtistProfile>> GetAllArtistProfile()
        {
            return await _context.ArtistProfiles.ToListAsync();
        }
        public async Task<ArtistProfile?> GetArtistProfileById(string id)
        {
            return await _context.ArtistProfiles.FirstOrDefaultAsync();
        }
        public async Task<ArtistProfile?> GetArtistProfileByName(string name)
        {
            return await _context.ArtistProfiles.FirstOrDefaultAsync(u => u.Name == name);
        }
        public void addArtistProfile(ArtistProfile artistProfile)
        {
            _context.ArtistProfiles.Add(artistProfile);
            _context.SaveChanges();
        }
        public void updateArtistProfile(ArtistProfile artistProfile)
        {
            _context.ArtistProfiles.Update(artistProfile);
            _context.SaveChanges();
        }
        public void deleteArtistProfile(ArtistProfile artistProfile)
        {
            _context.ArtistProfiles.Remove(artistProfile);
            _context.SaveChanges();
        }

        //ArtistFollow
        public async Task<ArtistFollow?> FollowArtist(int userId, string artistId)
        {
            // Verificar si ya sigue al artista
            var existingFollow = await _context.ArtistFollows
                .FirstOrDefaultAsync(af => af.UserId == userId && af.ArtistProfileId == artistId);

            if (existingFollow != null)
            {
                // Ya lo sigue, retornar null o el existente
                return null;
            }

            // Crear nuevo follow
            var artistFollow = new ArtistFollow
            {
                UserId = userId,
                ArtistProfileId = artistId,
                FollowDate = DateTime.UtcNow
            };

            _context.ArtistFollows.Add(artistFollow);
            await _context.SaveChangesAsync();

            return artistFollow;
        }

        public async Task<bool> UnfollowArtist(int userId, string artistId)
        {
            var artistFollow = await _context.ArtistFollows
                .FirstOrDefaultAsync(af => af.UserId == userId && af.ArtistProfileId == artistId);

            if (artistFollow == null)
            {
                return false; // No estaba siguiendo al artista
            }

            _context.ArtistFollows.Remove(artistFollow);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsFollowingArtist(int userId, string artistId)
        {
            return await _context.ArtistFollows
                .AnyAsync(af => af.UserId == userId && af.ArtistProfileId == artistId);
        }

        public async Task<List<ArtistProfile>> GetUserFollowedArtists(int userId)
        {
            return await _context.ArtistFollows
                .Where(af => af.UserId == userId)
                .Include(af => af.ArtistProfile)
                .OrderByDescending(af => af.FollowDate)
                .Select(af => af.ArtistProfile)
                .ToListAsync();
        }

        public async Task<int> GetArtistFollowersCount(string artistId)
        {
            return await _context.ArtistFollows
                .CountAsync(af => af.ArtistProfileId == artistId);
        }

        public async Task<List<User>> GetArtistFollowers(string artistId)
        {
            return await _context.ArtistFollows
                .Where(af => af.ArtistProfileId == artistId)
                .Include(af => af.User)
                .OrderByDescending(af => af.FollowDate)
                .Select(af => af.User)
                .ToListAsync();
        }
        //AlbumProfile
        public async Task<List<AlbumProfile>> GetAllAlbumProfile()
        {
            return await _context.AlbumProfiles.ToListAsync();
        }
        public async Task<AlbumProfile?> GetAlbumProfileById(string id)
        {
            return await _context.AlbumProfiles.FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<AlbumProfile?> GetAlbumProfileByName(string name)
        {
            return await _context.AlbumProfiles.FirstOrDefaultAsync(a=> a.Name == name);
        }
        public void addAlbumProfile(AlbumProfile albumProfile)
        {
            _context.AlbumProfiles.Add(albumProfile);
            _context.SaveChanges();
        }
        public void updateAlbumProfile(AlbumProfile albumProfile)
        {
            _context.AlbumProfiles.Update(albumProfile);
            _context.SaveChanges();
        }
        public void deleteAlbumProfile(AlbumProfile albumProfile)
        {
            _context.AlbumProfiles.Remove(albumProfile);
            _context.SaveChanges();
        }


        //SongProfile
        public async Task<List<SongProfile>> GetAllSongProfile()
        {
            return await _context.SongProfiles.ToListAsync();
        }
        public async Task<SongProfile?> GetSongProfileById(string id)
        {
            return await _context.SongProfiles.FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<SongProfile?> GetSongProfileByName(string name)
        {
            return await _context.SongProfiles.FirstOrDefaultAsync(s=> s.Name == name);
        }
        public void addSongProfile(SongProfile songProfile)
        {
            _context.SongProfiles.Add(songProfile);
            _context.SaveChanges();
        }
        public void updateSongProfile(SongProfile songProfile)
        {
            _context.SongProfiles.Update(songProfile);
            _context.SaveChanges();

        }
        public void deleteSongProfile(SongProfile songProfile)
        {
            _context.SongProfiles.Remove(songProfile);
            _context.SaveChanges();
        }


        //Reviews
        public async Task<List<Review>> GetAllReviews()
        {
            return await _context.Reviews.ToListAsync();
        }
        public async Task<Review?> GetReviewById(int id)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        }
        public void addReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }
        //public async Task addReview(Models.Review review)
        //{
        //    // 1. Agrega la review al "paquete" de cambios
        //    _context.Reviews.Add(review);

        //    // 2. ¡ESTA ES LA LÍNEA MÁGICA QUE FALTABA!
        //    //    Envía el "paquete" de cambios a la base de datos de Supabase
        //    //    y ESPERA a que termine.
        //    await _context.SaveChangesAsync();
        //}
        public void updateReview(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        public void deleteReview(Review review)
        {
            _context.Reviews.Remove(review);
            _context.SaveChanges();
        }

        public async Task<(Review? review, LikeType newStatus)> ToggleLike(int reviewId, int userId)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null) return (null, LikeType.None);

            var existingLike = await _context.ReviewLikes
                .FirstOrDefaultAsync(rl => rl.ReviewId == reviewId && rl.UserId == userId);

            LikeType finalStatus;


            if (existingLike != null)
            {
                if (existingLike.LikeType == LikeType.Like)
                {
                    // Si ya tenia like lo quita
                    review.Likes--;
                    _context.ReviewLikes.Remove(existingLike);
                    finalStatus = LikeType.None;
                }
                else
                {
                    // tenia dislike lo cambia a like
                    if (existingLike.LikeType == LikeType.Dislike) 
                    {
                        review.Dislikes--;
                    }
                    review.Likes++;
                    existingLike.LikeType = LikeType.Like;
                    existingLike.UpdatedAt = DateTime.UtcNow;
                    _context.ReviewLikes.Update(existingLike);
                    finalStatus = LikeType.Like;
                }
            }
            else
            {
                // si no existe le da like
                review.Likes++;
                var newLike = new ReviewLike
                {
                    ReviewId = reviewId,
                    UserId = userId,
                    LikeType = LikeType.Like,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ReviewLikes.Add(newLike);
                finalStatus = LikeType.Like;
            }

            await _context.SaveChangesAsync();
            return (review, finalStatus);
        }

        //Para dislike copy y paste pero cambiando el like por dislike
        public async Task<(Review? review, LikeType newStatus)> ToggleDislike(int reviewId, int userId)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null) return (null, LikeType.None);

            var existingDislike = await _context.ReviewLikes
                .FirstOrDefaultAsync(rl => rl.ReviewId == reviewId && rl.UserId == userId);

            LikeType finalStatus;

            if (existingDislike != null)
            {
                if (existingDislike.LikeType == LikeType.Dislike)
                {
                    // Si ya tenia dislike lo quita
                    review.Dislikes--;
                    _context.ReviewLikes.Remove(existingDislike);
                    finalStatus= LikeType.None;
                }
                else 
                {
                    // los mismo que el otro pero con dislike
                    if (existingDislike.LikeType == LikeType.Like)
                    {
                        review.Likes--;
                    }
                    review.Dislikes++;
                    existingDislike.LikeType = LikeType.Dislike;
                    existingDislike.UpdatedAt = DateTime.UtcNow;
                    _context.ReviewLikes.Update(existingDislike);
                    finalStatus = LikeType.Dislike;
                }
            }
            else
            {
                // si no existe le da like
                review.Dislikes++;
                var newDislike = new ReviewLike
                {
                    ReviewId = reviewId,
                    UserId = userId,
                    LikeType = LikeType.Dislike,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.ReviewLikes.Add(newDislike);
                finalStatus = LikeType.Dislike;
            }

            await _context.SaveChangesAsync();
            return (review,finalStatus);
        }

        //Saber que tiene (like, dislike o nada)
        public async Task<LikeType> GetUserLikeStatus(int reviewId, int userId)
        {
            var reviewLike = await _context.ReviewLikes
                .FirstOrDefaultAsync(rl => rl.ReviewId == reviewId && rl.UserId == userId);

            return reviewLike?.LikeType ?? LikeType.None;
        }

        //ReviewComments
        public async Task<List<ReviewComment>> GetAllReviewComments()
        {
            return await _context.ReviewComments.ToListAsync();
        }
        public async Task<ReviewComment?> GetReviewCommentById(int id)
        {
            return await _context.ReviewComments.FirstOrDefaultAsync(rc => rc.Id == id);
        }
        public void addReviewComment(ReviewComment reviewComment)
        {
            _context.ReviewComments.Add(reviewComment);
            _context.SaveChanges();
        }
        public void updateReviewComment(ReviewComment reviewComment)
        {
            _context.ReviewComments.Update(reviewComment);
            _context.SaveChanges();
        }
        public void deleteReviewComment(ReviewComment reviewComment)
        {
            _context.ReviewComments.Remove(reviewComment);
            _context.SaveChanges();
        }

       

    }
}
