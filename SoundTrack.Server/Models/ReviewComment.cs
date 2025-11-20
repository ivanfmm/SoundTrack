namespace SoundTrack.Server.Models
{
    public class ReviewComment
    {
        public int Id { get; set; }

        // Foreign Key para Review
        public int ReviewId { get; set; }
        public Review Review { get; set; }

        // Foreign Key para User (string porque viene de Identity)
        public string UserId { get; set; }
        public User User { get; set; }

        // Contenido del comentario
        public string Content { get; set; }

        // Fechas
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}