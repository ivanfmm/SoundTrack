using Microsoft.EntityFrameworkCore;
using System;
using System.IO;


namespace SoundTrack.Server.Data
{
    public class SoundTrackContext : DbContext
    {
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.ArtistProfile> ArtistProfiles { get; set; }
        public DbSet<Models.AlbumProfile> AlbumProfiles { get; set; }
        public DbSet<Models.SongProfile> SongProfiles { get; set; }
        public DbSet<Models.Review> Reviews { get; set; }
        public DbSet<Models.ReviewComment> ReviewComments { get; set; }
        public DbSet<Models.ReviewLike> ReviewLikes { get; set; }

        public SoundTrackContext(DbContextOptions<SoundTrackContext> options) : base(options)
        {
            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);
            //DbPath = Path.Join(path, "soundtrack.db");
        }
    }
}
