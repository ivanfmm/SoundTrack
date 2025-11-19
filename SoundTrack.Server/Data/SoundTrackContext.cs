using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoundTrack.Server.Models;

namespace SoundTrack.Server.Data
{
    public class SoundTrackContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ArtistProfile> ArtistProfiles { get; set; }
        public DbSet<AlbumProfile> AlbumProfiles { get; set; }
        public DbSet<SongProfile> SongProfiles { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }
        public DbSet<ReviewLike> ReviewLikes { get; set; }
        public DbSet<Models.ArtistFollow> ArtistFollows { get; set; }


        public SoundTrackContext(DbContextOptions<SoundTrackContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CRÍTICO: Llamar al método base para que Identity configure sus tablas
            base.OnModelCreating(modelBuilder);

            // Configuración básica de User - agregar el campo para vincular con Identity
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.IdentityUserId)
                    .HasMaxLength(450)
                    .IsRequired(false);

                // Índice único para email y username si aún no lo tienes
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // EF Core detectará automáticamente las demás configuraciones
            // basándose en las convenciones de tus modelos existentes
        }
    }
}