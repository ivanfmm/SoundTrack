using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundTrack.Server.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(UserId))]  // ⭐ Use nameof
        public User User { get; set; }

        public string? ArtistProfileId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ArtistProfileId))]  // ⭐ Use nameof
        public ArtistProfile? ArtistProfile { get; set; }

        public string? AlbumProfileId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(AlbumProfileId))]  // ⭐ Use nameof
        public AlbumProfile? AlbumProfile { get; set; }

        public string? SongProfileId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(SongProfileId))]  // ⭐ Use nameof
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