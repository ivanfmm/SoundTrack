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
            _context.SaveChangesAsync();
        }
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChangesAsync();
        }
        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
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
            _context.SaveChangesAsync();
        }
        public void updateArtistProfile(ArtistProfile artistProfile)
        {
            _context.ArtistProfiles.Update(artistProfile);
            _context.SaveChangesAsync();
        }
        public void deleteArtistProfile(ArtistProfile artistProfile)
        {
            _context.ArtistProfiles.Remove(artistProfile);
            _context.SaveChangesAsync();
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
            _context.SaveChangesAsync();
        }
        public void updateAlbumProfile(AlbumProfile albumProfile)
        {
            _context.AlbumProfiles.Update(albumProfile);
            _context.SaveChangesAsync();
        }
        public void deleteAlbumProfile(AlbumProfile albumProfile)
        {
            _context.AlbumProfiles.Remove(albumProfile);
            _context.SaveChangesAsync();
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
            _context.SaveChangesAsync();
        }
        public void updateSongProfile(SongProfile songProfile)
        {
            _context.SongProfiles.Update(songProfile);
            _context.SaveChangesAsync();

        }
        public void deleteSongProfile(SongProfile songProfile)
        {
            _context.SongProfiles.Remove(songProfile);
            _context.SaveChangesAsync();
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
            _context.SaveChangesAsync();
        }
        public void updateReview(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChangesAsync();
        }

        public void deleteReview(Review review)
        {
            _context.Reviews.Remove(review);
            _context.SaveChangesAsync();
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
            _context.SaveChangesAsync();
        }
        public void updateReviewComment(ReviewComment reviewComment)
        {
            _context.ReviewComments.Update(reviewComment);
            _context.SaveChangesAsync();
        }
        public void deleteReviewComment(ReviewComment reviewComment)
        {
            _context.ReviewComments.Remove(reviewComment);
            _context.SaveChangesAsync();
        }



    }
}
