using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoundTrack.Server.Models;

namespace SoundTrack.Server.Data
{
	public class SoundTrackContext : IdentityDbContext<User>
	{
		public DbSet<ArtistProfile> ArtistProfiles { get; set; }
		public DbSet<AlbumProfile> AlbumProfiles { get; set; }
		public DbSet<SongProfile> SongProfiles { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<ReviewComment> ReviewComments { get; set; }
		public DbSet<ReviewLike> ReviewLikes { get; set; }
		public DbSet<ArtistFollow> ArtistFollows { get; set; }
		public DbSet<UserUser> UserFollows { get; set; }

		public SoundTrackContext(DbContextOptions<SoundTrackContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//Configuracion user
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasIndex(e => e.UserName).IsUnique();

				entity.HasMany(u => u.Followers)
					.WithOne(uf => uf.Following)
					.HasForeignKey(uf => uf.FollowingId)
					.OnDelete(DeleteBehavior.Restrict);

				entity.HasMany(u => u.Following)
					.WithOne(uf => uf.Follower)
					.HasForeignKey(uf => uf.FollowerId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// configuracion User User
			modelBuilder.Entity<UserUser>(entity =>
			{
				entity.ToTable("UserUser");
				entity.HasKey(uf => uf.Id);
				entity.HasIndex(uf => new { uf.FollowerId, uf.FollowingId }).IsUnique();
				entity.Property(uf => uf.FollowDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
			});

			//Configuracion review
			modelBuilder.Entity<Review>(entity =>
			{
				entity.HasKey(r => r.Id);

				// Propiedades
				entity.Property(r => r.UserId).IsRequired();
				entity.Property(r => r.Title).IsRequired();
				entity.Property(r => r.Content).IsRequired();
				entity.Property(r => r.Likes).HasDefaultValue(0);
				entity.Property(r => r.Dislikes).HasDefaultValue(0);
				entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

				// Relacion con user
				entity.HasOne(r => r.User)
					.WithMany(u => u.Reviews)
					.HasForeignKey(r => r.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				// Relaciones con profiles
				entity.HasOne(r => r.ArtistProfile)
					.WithMany(a => a.reviews)
					.HasForeignKey(r => r.ArtistProfileId)
					.OnDelete(DeleteBehavior.SetNull)
					.IsRequired(false);

				entity.HasOne(r => r.AlbumProfile)
					.WithMany(a => a.reviews)
					.HasForeignKey(r => r.AlbumProfileId)
					.OnDelete(DeleteBehavior.SetNull)
					.IsRequired(false);

				entity.HasOne(r => r.SongProfile)
					.WithMany(s => s.reviews)
					.HasForeignKey(r => r.SongProfileId)
					.OnDelete(DeleteBehavior.SetNull)
					.IsRequired(false);

				// indices
				entity.HasIndex(r => r.UserId);
				entity.HasIndex(r => r.ArtistProfileId);
				entity.HasIndex(r => r.AlbumProfileId);
				entity.HasIndex(r => r.SongProfileId);
				entity.HasIndex(r => r.CreatedAt);
			});

			// Configuracion review coment
			modelBuilder.Entity<ReviewComment>(entity =>
			{
				entity.HasKey(rc => rc.Id);

				entity.HasOne(rc => rc.Review)
					.WithMany(r => r.Comments)
					.HasForeignKey(rc => rc.ReviewId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(rc => rc.User)
					.WithMany()
					.HasForeignKey(rc => rc.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasIndex(rc => rc.ReviewId);
				entity.HasIndex(rc => rc.UserId);
			});

			// Configuracion review like
			modelBuilder.Entity<ReviewLike>(entity =>
			{
				entity.HasKey(rl => rl.Id);
				entity.HasIndex(rl => new { rl.ReviewId, rl.UserId }).IsUnique();

				entity.HasOne(rl => rl.Review)
					.WithMany(r => r.ReviewLikes)
					.HasForeignKey(rl => rl.ReviewId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(rl => rl.User)
					.WithMany(u => u.ReviewLikes)
					.HasForeignKey(rl => rl.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// Configuracion artist follow
			modelBuilder.Entity<ArtistFollow>(entity =>
			{
				entity.HasKey(af => af.Id);
				entity.HasIndex(af => new { af.UserId, af.ArtistProfileId }).IsUnique();

				entity.HasOne(af => af.User)
					.WithMany(u => u.FollowedArtists)
					.HasForeignKey(af => af.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(af => af.ArtistProfile)
					.WithMany()
					.HasForeignKey(af => af.ArtistProfileId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}