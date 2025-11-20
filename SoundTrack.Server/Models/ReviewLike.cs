namespace SoundTrack.Server.Models
{
    public class ReviewLike
    {
        public int Id { get; set; }

        // Foreign Key para Review
        public int ReviewId { get; set; }
        public Review Review { get; set; }

        // Foreign Key para User (string porque viene de Identity)
        public string UserId { get; set; }
        public User User { get; set; }

        // Tipo de like (Like o Dislike)
        public LikeType LikeType { get; set; }

        // Fechas
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public enum LikeType
    {
        None = 0,
        Like = 1,
        Dislike = 2
    }
}