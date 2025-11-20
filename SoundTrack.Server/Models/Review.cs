using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundTrack.Server.Models
{
    public class Review
    {
        public int Id { get; set; }

        // Foreign Key para User (Identity usa string)
        [Required]
        public string UserId { get; set; }

        // Propiedad de navegación hacia User
        [ValidateNever]
        public User User { get; set; }

        // Foreign Keys para los perfiles (nullables)
        public string? ArtistProfileId { get; set; }
        [ValidateNever]
        public ArtistProfile? ArtistProfile { get; set; }

        public string? AlbumProfileId { get; set; }
        [ValidateNever]
        public AlbumProfile? AlbumProfile { get; set; }

        public string? SongProfileId { get; set; }
        [ValidateNever]
        public SongProfile? SongProfile { get; set; }

        // Contenido de la review
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public ReviewScore score { get; set; }

        // Contadores de likes/dislikes
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;

        // Fechas
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Colecciones de navegación
        [ValidateNever]
        public List<ReviewComment> Comments { get; set; } = new List<ReviewComment>();

        [ValidateNever]
        public List<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
    }

    // Enum para el sistema de puntuación
    public enum ReviewScore
    {
        OneStar = 1,
        TwoStars = 2,
        ThreeStars = 3,
        FourStars = 4,
        FiveStars = 5
    }
}