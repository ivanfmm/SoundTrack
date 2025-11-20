// Controllers/UserController.cs
using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Services;
using SoundTrack.Server.DTOs;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly ISoundTrackRepository _repository;

        public UserController(ISoundTrackRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}/profile")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(string id)
        {
            var profile = await _repository.GetUserProfileWithStatsAsync(id);

            if (profile == null)
            {
                return NotFound(new { error = "Usuario no encontrado" });
            }

            return Ok(profile);
        }

        [HttpPatch("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateProfileRequest request)
        {
            var success = await _repository.UpdateUserProfileAsync(
                id,
                request.Username,
                request.Email,
                request.Bio
            );

            if (!success)
            {
                return NotFound(new { error = "Usuario no encontrado" });
            }

            return Ok(new { message = "Perfil actualizado" });
        }

        [HttpPut("{id}/favorites")]
        public async Task<IActionResult> UpdateFavorites(string id, [FromBody] UpdateFavoritesRequest request)
        {
            var success = await _repository.UpdateUserFavoritesAsync(
                id,
                request.FavoriteArtistIds,
                request.FavoriteAlbumIds,
                request.FavoriteSongIds
            );

            if (!success)
            {
                return NotFound(new { error = "Usuario no encontrado" });
            }

            return Ok(new { message = "Favoritos actualizados" });
        }



        // Request models
        public class UpdateProfileRequest
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Bio { get; set; }
        }

        public class UpdateFavoritesRequest
        {
            public string? FavoriteArtistIds { get; set; }
            public string? FavoriteAlbumIds { get; set; }
            public string? FavoriteSongIds { get; set; }
        }
    }
}