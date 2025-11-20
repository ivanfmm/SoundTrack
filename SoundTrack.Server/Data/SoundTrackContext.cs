using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoundTrack.Server.Models;

namespace SoundTrack.Server.Data
{
    public class SoundTrackContext : IdentityDbContext<User>
    {
        //public DbSet<User> Users { get; set; }
        public DbSet<ArtistProfile> ArtistProfiles { get; set; }
        public DbSet<AlbumProfile> AlbumProfiles { get; set; }
        public DbSet<SongProfile> SongProfiles { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }
        public DbSet<ReviewLike> ReviewLikes { get; set; }
        public DbSet<Models.ArtistFollow> ArtistFollows { get; set; }
        public DbSet<UserUser> UserFollows { get; set; }


        public SoundTrackContext(DbContextOptions<SoundTrackContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CRÍTICO: Llamar primero al método base
            base.OnModelCreating(modelBuilder);

            // ===== CONFIGURACIÓN DE USER =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.UserName).IsUnique();

                // Relaciones con UserFollow
                entity.HasMany(u => u.Followers)
                    .WithOne(uf => uf.Following)
                    .HasForeignKey(uf => uf.FollowingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Following)
                    .WithOne(uf => uf.Follower)
                    .HasForeignKey(uf => uf.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CONFIGURACIÓN DE USERFOLLOWS =====
            modelBuilder.Entity<UserUser>(entity =>
            {
                // ⭐ AGREGADO: Mapear a la tabla "UserUser" existente
                entity.ToTable("UserUser");

                entity.HasKey(uf => uf.Id);
                entity.HasIndex(uf => new { uf.FollowerId, uf.FollowingId }).IsUnique();
                entity.Property(uf => uf.FollowDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // ===== CONFIGURACIÓN DE REVIEW =====
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);

                // Relación con User
                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con ArtistProfile (opcional)
                entity.HasOne(r => r.ArtistProfile)
                    .WithMany()
                    .HasForeignKey(r => r.ArtistProfileId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con AlbumProfile (opcional)
                entity.HasOne(r => r.AlbumProfile)
                    .WithMany()
                    .HasForeignKey(r => r.AlbumProfileId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con SongProfile (opcional)
                entity.HasOne(r => r.SongProfile)
                    .WithMany()
                    .HasForeignKey(r => r.SongProfileId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Índices para búsquedas rápidas
                entity.HasIndex(r => r.UserId);
                entity.HasIndex(r => r.ArtistProfileId);
                entity.HasIndex(r => r.AlbumProfileId);
                entity.HasIndex(r => r.SongProfileId);
                entity.HasIndex(r => r.CreatedAt);
            });

            // ===== CONFIGURACIÓN DE REVIEWCOMMENT =====
            modelBuilder.Entity<ReviewComment>(entity =>
            {
                entity.HasKey(rc => rc.Id);

                // Relación con Review
                entity.HasOne(rc => rc.Review)
                    .WithMany(r => r.Comments)
                    .HasForeignKey(rc => rc.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con User
                entity.HasOne(rc => rc.User)
                    .WithMany()
                    .HasForeignKey(rc => rc.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(rc => rc.ReviewId);
                entity.HasIndex(rc => rc.UserId);
            });

            // ===== CONFIGURACIÓN DE REVIEWLIKE =====
            modelBuilder.Entity<ReviewLike>(entity =>
            {
                entity.HasKey(rl => rl.Id);

                // Índice único: un usuario solo puede dar like/dislike una vez por review
                entity.HasIndex(rl => new { rl.ReviewId, rl.UserId }).IsUnique();

                // Relación con Review
                entity.HasOne(rl => rl.Review)
                    .WithMany(r => r.ReviewLikes)
                    .HasForeignKey(rl => rl.ReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con User
                entity.HasOne(rl => rl.User)
                    .WithMany(u => u.ReviewLikes)
                    .HasForeignKey(rl => rl.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== CONFIGURACIÓN DE ARTISTFOLLOW =====
            modelBuilder.Entity<ArtistFollow>(entity =>
            {
                entity.HasKey(af => af.Id);

                // Índice único: un usuario solo puede seguir un artista una vez
                entity.HasIndex(af => new { af.UserId, af.ArtistProfileId }).IsUnique();

                // Relación con User
                entity.HasOne(af => af.User)
                    .WithMany(u => u.FollowedArtists)
                    .HasForeignKey(af => af.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con ArtistProfile
                entity.HasOne(af => af.ArtistProfile)
                    .WithMany()
                    .HasForeignKey(af => af.ArtistProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}