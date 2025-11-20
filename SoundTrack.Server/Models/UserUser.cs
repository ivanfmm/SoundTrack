namespace SoundTrack.Server.Models
{
    public class UserUser
    {
        public int Id { get; set; }

        // Usuario que sigue
        public string FollowerId { get; set; }
        public User Follower { get; set; }

        // Usuario seguido
        public string FollowingId { get; set; }
        public User Following { get; set; }

        public DateTime FollowDate { get; set; }
    }
}
