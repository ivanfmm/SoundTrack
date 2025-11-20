using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Services;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("api/user")]

    public class UserUserController : ControllerBase
    {
        private readonly ISoundTrackRepository _repository;

        public UserUserController(ISoundTrackRepository repository)
        {
            _repository = repository;
        }

        // GET: api/user/{userId}/is-following/{targetUserId}
        [HttpGet("{userId}/is-following/{targetUserId}")]
        public async Task<IActionResult> IsFollowing(string userId, string targetUserId)
        {
            try
            {
                var isFollowing = await _repository.IsFollowingAsync(userId, targetUserId);
                return Ok(new { isFollowing });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/user/{userId}/follow/{targetUserId}
        [HttpPost("{userId}/follow/{targetUserId}")]
        public async Task<IActionResult> FollowUser(string userId, string targetUserId)
        {
            try
            {
                if (userId == targetUserId)
                {
                    return BadRequest(new { error = "No puedes seguirte a ti mismo" });
                }

                var result = await _repository.FollowUserAsync(userId, targetUserId);

                if (!result)
                {
                    return BadRequest(new { error = "Ya sigues a este usuario" });
                }

                return Ok(new { message = "Usuario seguido exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE: api/user/{userId}/unfollow/{targetUserId}
        [HttpDelete("{userId}/unfollow/{targetUserId}")]
        public async Task<IActionResult> UnfollowUser(string userId, string targetUserId)
        {
            try
            {
                var result = await _repository.UnfollowUserAsync(userId, targetUserId);

                if (!result)
                {
                    return BadRequest(new { error = "No sigues a este usuario" });
                }

                return Ok(new { message = "Dejaste de seguir al usuario" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}