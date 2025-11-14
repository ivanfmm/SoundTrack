using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Data;
using SoundTrack.Server.Services;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly ISoundTrackRepository _SoundTrackRepository;
        private readonly ISpotifyProfileService _spotifyProfileService;

        public ReviewController(ISoundTrackRepository SoundTrackRepository, ISpotifyProfileService spotifyProfileService)
        {
            _SoundTrackRepository = SoundTrackRepository;
            _spotifyProfileService = spotifyProfileService;
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
        public async Task<IActionResult> AddReview([FromBody] Models.Review review)
        {
            try
            {
                // ⭐ VERIFICAR Y CREAR PROFILES SI NO EXISTEN

                if (!string.IsNullOrEmpty(review.SongProfileId))
                {
                    // Asegurar que el SongProfile existe (y sus dependencias)
                    await _spotifyProfileService.EnsureSongProfileExists(review.SongProfileId);
                }
                else if (!string.IsNullOrEmpty(review.ArtistProfileId))
                {
                    // Asegurar que el ArtistProfile existe
                    await _spotifyProfileService.EnsureArtistProfileExists(review.ArtistProfileId);
                }
                else if (!string.IsNullOrEmpty(review.AlbumProfileId))
                {
                    // Asegurar que el AlbumProfile existe
                    await _spotifyProfileService.EnsureAlbumProfileExists(review.AlbumProfileId);
                }

                // Ahora sí, crear la review
                _SoundTrackRepository.addReview(review);

                return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    error = "Error al crear la review", 
                    message = ex.Message 
                });
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> AddReview([FromBody] Models.Review review)
        //{
        //    await _SoundTrackRepository.addReview(review);
        //    return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        //}
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
