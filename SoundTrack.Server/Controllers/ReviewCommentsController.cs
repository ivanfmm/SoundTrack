using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Services;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("[ReviewComments]")]
    public class ReviewCommentsController : Controller
    {
        private readonly ISoundTrackRepository _SoundTrackRepository;
        public ReviewCommentsController(ISoundTrackRepository SoundTrackRepository)
        {
            _SoundTrackRepository = SoundTrackRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviewComments()
        {
            var reviewComments = await _SoundTrackRepository.GetAllReviewComments();
            return Ok(reviewComments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewCommentById(int id)
        {
            var reviewComment = await _SoundTrackRepository.GetReviewCommentById(id);
            if (reviewComment == null)
            {
                return NotFound();
            }
            return Ok(reviewComment);
        }
        [HttpPost]
        public IActionResult AddReviewComment([FromBody] Models.ReviewComment reviewComment)
        {
            _SoundTrackRepository.addReviewComment(reviewComment);
            return CreatedAtAction(nameof(GetReviewCommentById), new { id = reviewComment.Id }, reviewComment);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateReviewComment(int id, [FromBody] Models.ReviewComment reviewComment)
        {
            if (id != reviewComment.Id)
            {
                return BadRequest();
            }
            var existingReviewComment = _SoundTrackRepository.GetReviewCommentById(id).Result;
            if (existingReviewComment == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.updateReviewComment(reviewComment);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteReviewComment(int id)
        {
            var existingReviewComment = _SoundTrackRepository.GetReviewCommentById(id).Result;
            if (existingReviewComment == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.deleteReviewComment(existingReviewComment);
            return NoContent();
        }
    }
}
