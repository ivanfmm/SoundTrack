using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundTrack.Server.Models
{
    public class Review
    {
        public int Id { get; set; }

        // Foreign Key para User (Identity usa string)
        public string UserId { get; set; }

        // Propiedad de navegación hacia User
        [ValidateNever] // ← Añade esto
        [ForeignKey("UserId")]
        public User User { get; set; }

        // Foreign Keys para los perfiles (pueden ser nullables si no siempre se asocian)
        public string? ArtistProfileId { get; set; }
        [ForeignKey("ArtistProfileId")]
        public ArtistProfile? ArtistProfile { get; set; }

        public string? AlbumProfileId { get; set; }
        [ForeignKey("AlbumProfileId")]
        public AlbumProfile? AlbumProfile { get; set; }

        public string? SongProfileId { get; set; }
        public SongProfile? SongProfile { get; set; }

        // Contenido de la review
        public string Title { get; set; }
        public string Content { get; set; }
        public ReviewScore score { get; set; }  // Enum para las estrellas

        // Contadores de likes/dislikes
        public int Likes { get; set; }
        public int Dislikes { get; set; }

        // Fechas
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Colecciones de navegación
        public List<ReviewComment> Comments { get; set; } = new List<ReviewComment>();
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