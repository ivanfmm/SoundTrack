namespace SoundTrack.Server.Models
{
    public abstract class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }

        public void Like()
        {
            Likes++;
        }
        public void Dislike()
        {
            Dislikes++;
        }
    }
}
