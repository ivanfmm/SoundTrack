using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using SoundTrack.Server.Data;
using SoundTrack.Server.Services;
using System.Configuration;
namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistProfileController : Controller
    {
        private readonly ISoundTrackRepository _SoundTrackRepository;
        public ArtistProfileController(ISoundTrackRepository SoundTrackRepository)
        {
            _SoundTrackRepository = SoundTrackRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArtistProfile()
        {
            var artistProfiles = await _SoundTrackRepository.GetAllArtistProfile();
            return Ok(artistProfiles);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArtistProfileById(string id)
        {
            var artistProfile = await _SoundTrackRepository.GetArtistProfileById(id);
            if (artistProfile == null)
            {
                return NotFound();
            }
            return Ok(artistProfile);
        }
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetArtistProfileByName(string name)
        {
            var artistProfile = await _SoundTrackRepository.GetArtistProfileByName(name);
            if (artistProfile == null)
            {
                return NotFound();
            }
            return Ok(artistProfile);
        }
        [HttpPost]
        public IActionResult AddArtistProfile([FromBody] Models.ArtistProfile artistProfile)
        {
            _SoundTrackRepository.addArtistProfile(artistProfile);
            return CreatedAtAction(nameof(GetArtistProfileById), new { id = artistProfile.Id }, artistProfile);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateArtistProfile(string id, [FromBody] Models.ArtistProfile artistProfile)
        {
            if (id != artistProfile.Id)
            {
                return BadRequest();
            }
            var existingArtistProfile = _SoundTrackRepository.GetArtistProfileById(id).Result;
            if (existingArtistProfile == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.updateArtistProfile(artistProfile);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteArtistProfile(string id)
        {
            var existingArtistProfile = _SoundTrackRepository.GetArtistProfileById(id).Result;
            if (existingArtistProfile == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.deleteArtistProfile(existingArtistProfile);
            return NoContent();
        }
        [HttpPost("{artistId}/follow")]

        // Seguir a un artista
        public async Task<IActionResult> FollowArtist(string artistId, [FromBody] FollowRequest request)
        {
            if (string.IsNullOrEmpty(artistId))
            {
                return BadRequest("Artist ID is required");
            }

            // Verificar que el artista existe
            var artist = await _SoundTrackRepository.GetArtistProfileById(artistId);
            if (artist == null)
            {
                return NotFound("Artist not found");
            }

            // Verificar que el usuario existe
            var user = await _SoundTrackRepository.GetUserById(request.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Intentar seguir al artista
            var artistFollow = await _SoundTrackRepository.FollowArtist(request.UserId, artistId);

            if (artistFollow == null)
            {
                return BadRequest("You are already following this artist");
            }

            // Obtener el conteo actualizado de followers
            var followersCount = await _SoundTrackRepository.GetArtistFollowersCount(artistId);

            return Ok(new
            {
                message = "Successfully followed artist",
                artistId = artistId,
                userId = request.UserId,
                followDate = artistFollow.FollowDate,
                followersCount = followersCount
            });
        }

        // Deja de seguir al artista
        [HttpDelete("{artistId}/unfollow")]
        public async Task<IActionResult> UnfollowArtist(string artistId, [FromBody] FollowRequest request)
        {
            if (string.IsNullOrEmpty(artistId))
            {
                return BadRequest("Artist ID is required");
            }

            var success = await _SoundTrackRepository.UnfollowArtist(request.UserId, artistId);

            if (!success)
            {
                return NotFound("You are not following this artist");
            }

            // Obtener el conteo actualizado de followers
            var followersCount = await _SoundTrackRepository.GetArtistFollowersCount(artistId);

            return Ok(new
            {
                message = "Successfully unfollowed artist",
                artistId = artistId,
                userId = request.UserId,
                followersCount = followersCount
            });
        }

        // Verifica si ya sigue el artista
        [HttpGet("{artistId}/is-following/{userId}")]
        public async Task<IActionResult> IsFollowingArtist(string artistId, int userId)
        {
            var isFollowing = await _SoundTrackRepository.IsFollowingArtist(userId, artistId);

            return Ok(new
            {
                artistId = artistId,
                userId = userId,
                isFollowing = isFollowing
            });
        }

        // Obtener conteo de followers de un artista
        [HttpGet("{artistId}/followers-count")]
        public async Task<IActionResult> GetFollowersCount(string artistId)
        {
            var count = await _SoundTrackRepository.GetArtistFollowersCount(artistId);

            return Ok(new
            {
                artistId = artistId,
                followersCount = count
            });
        }

        // Obtener lista de usuarios que siguen a un artista
        [HttpGet("{artistId}/followers")]
        public async Task<IActionResult> GetArtistFollowers(string artistId)
        {
            var artist = await _SoundTrackRepository.GetArtistProfileById(artistId);
            if (artist == null)
            {
                return NotFound("Artist not found");
            }

            var followers = await _SoundTrackRepository.GetArtistFollowers(artistId);

            return Ok(new
            {
                artistId = artistId,
                artistName = artist.Name,
                followersCount = followers.Count,
                followers = followers.Select(u => new
                {
                    id = u.Id,
                    username = u.Username,
                    profilePictureUrl = u.ProfilePictureUrl
                })
            });
        }

        // Obtener artistas que sigue un usuario
        [HttpGet("followed/user/{userId}")]
        public async Task<IActionResult> GetUserFollowedArtists(int userId)
        {
            var user = await _SoundTrackRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var followedArtists = await _SoundTrackRepository.GetUserFollowedArtists(userId);

            return Ok(new
            {
                userId = userId,
                username = user.Username,
                followedArtistsCount = followedArtists.Count,
                followedArtists = followedArtists.Select(a => new
                {
                    id = a.Id,
                    name = a.Name,
                    imageUrl = a.ImageUrl,
                    score = a.score
                })
            });
        }
    }

    // Clase auxiliar para recibir el userId en el body
    public class FollowRequest
    {
        public int UserId { get; set; }
    }
}

