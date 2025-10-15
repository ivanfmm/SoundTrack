namespace SoundTrack.Server.Models
{
    public abstract class Profile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public Score score { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<Review> reviews { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }

        public void addReview(Review review)
        {
            if (reviews == null)
            {
                reviews = new List<Review>();
            }
            reviews.Add(review);
        }

        public void calculateScore()
        {
            if (reviews == null || reviews.Count == 0)
            {
                score = Score.OneStar;
                return;
            }
            int totalScore = 0;
            foreach (var review in reviews)
            {
                totalScore += (int)review.score;
            }
            int averageScore = totalScore / reviews.Count;
            score = (Score)Math.Min(Math.Max(averageScore, 1), 5);
        }
    }
}
