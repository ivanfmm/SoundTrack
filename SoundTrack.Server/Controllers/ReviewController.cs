using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Data;
using SoundTrack.Server.Models;
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
                // Revisa si los perfiles existen

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

        [HttpPut("{id}/like")]
        public async Task<IActionResult> ToggleLike(int id, [FromBody] UserActionRequest request)
        {
            var(review, userStatus) = await _SoundTrackRepository.ToggleLike(id, request.UserId);

            if (review == null)
            {
                return NotFound();
            }

            var cleanReviewDto = new
            {
                // Propiedades que React necesita (SI SE BORRA DA ERROR 500)
                Id = review.Id,
                Author = review.User,
                Description = review.Content,
                Score = review.score,
                PublicationDate = review.CreatedAt,
                Likes = review.Likes,        
                Dislikes = review.Dislikes,   

                // Incluye estas por si acaso es recomendado
                SongProfileId = review.SongProfileId,
                ArtistProfileId = review.ArtistProfileId,
                AlbumProfileId = review.AlbumProfileId
            };

            return Ok(new
            {
                review = cleanReviewDto,
                userLikeStatus = userStatus.ToString()
            });
        }

        [HttpPut("{id}/dislike")]
        public async Task<IActionResult> ToggleDislike(int id, [FromBody] UserActionRequest request)
        {
            var (review, userStatus) = await _SoundTrackRepository.ToggleDislike(id, request.UserId);

            if (review == null)
            {
                return NotFound();
            }

            var cleanReviewDto = new
            {
                Id = review.Id,
                Author = review.User,
                Description = review.Content,
                Score = review.score,
                PublicationDate = review.CreatedAt,
                Likes = review.Likes,
                Dislikes = review.Dislikes,
                SongProfileId = review.SongProfileId,
                ArtistProfileId = review.ArtistProfileId,
                AlbumProfileId = review.AlbumProfileId
            };

            return Ok(new
            {
                review = cleanReviewDto,
                userLikeStatus = userStatus.ToString()
            });
        }

        [HttpGet("{id}/like-status/{userId}")]
        public async Task<IActionResult> GetUserLikeStatus(int id, string userId)
        {
            var status = await _SoundTrackRepository.GetUserLikeStatus(id, userId);
            return Ok(new
            {
                reviewId = id,
                userId = userId,
                likeStatus = status.ToString()
            });
        }

        [HttpGet("average-score/{profileId}")]
        public async Task<IActionResult> GetAverageScore(string profileId, [FromQuery] string profileType)
        {
            if (string.IsNullOrEmpty(profileType))
            {
                return BadRequest("profileType is required");
            }

            var averageScore = await _SoundTrackRepository.GetAverageScore(profileId, profileType);

            return Ok(new
            {
                profileId = profileId,
                profileType = profileType,
                averageScore = averageScore,
                totalReviews = averageScore.HasValue ?
                    await _SoundTrackRepository.GetReviewCountByProfile(profileId, profileType) : 0
            });
        }

    }

    public class UserActionRequest
    {
        public string UserId { get; set; }
    }
}
