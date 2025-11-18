namespace SoundTrack.Server.Models
{
    //Usamos un enum como en las esztrellas
    public enum LikeType
    {
        None = 0,
        Like = 1,
        Dislike = 2
    }
    public class ReviewLike
    {
        //Id unico (por si acaso)
        public int Id { get; set; }

        // Foreign Keys para vincular el usuario con el review
        public int ReviewId { get; set; }
        public int UserId { get; set; }

        // Navigation Properties (para poder buscarlo)
        public Review Review { get; set; }
        public User User { get; set; }

        // Tipo de accion ( nada (si lo llega a quitar) like o dislike)
        public LikeType LikeType { get; set; }

        // Fecha de la primera interaccion y si cambia algo
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
