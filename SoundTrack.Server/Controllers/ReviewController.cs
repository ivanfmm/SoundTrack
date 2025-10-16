using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Data;
using SoundTrack.Server.Services;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("[ReviewController]")]
    public class ReviewController : Controller
    {
        private readonly ISoundTrackRepository _SoundTrackRepository;
        public ReviewController(ISoundTrackRepository SoundTrackRepository)
        {
            _SoundTrackRepository = SoundTrackRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _SoundTrackRepository.GetAllReviews();
            return Ok(reviews);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _SoundTrackRepository.GetReviewById(id);
            if (review == null)
            {
                return NotFound();
            }
            return  Ok(review);
        }

        [HttpPost]
        public IActionResult AddReview([FromBody] Models.Review review)
        {
            _SoundTrackRepository.addReview(review);
            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateReview(int id, [FromBody] Models.Review review)
        {
            if (id != review.Id)
            {
                return BadRequest();
            }
            var existingReview = _SoundTrackRepository.GetReviewById(id).Result;
            if (existingReview == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.updateReview(review);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            var existingReview = _SoundTrackRepository.GetReviewById(id).Result;
            if (existingReview == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.deleteReview(existingReview);
            return NoContent();
        }
    }
}
