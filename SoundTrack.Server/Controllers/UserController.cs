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
    public class UserController : Controller
    {
        private readonly ISoundTrackRepository _SoundTrackRepository;
        public UserController(ISoundTrackRepository SoundTrackRepository)
        {
            _SoundTrackRepository = SoundTrackRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _SoundTrackRepository.GetAllUsers();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _SoundTrackRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _SoundTrackRepository.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] Models.User user)
        {
            _SoundTrackRepository.AddUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, [FromBody] Models.User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            var existingUser = _SoundTrackRepository.GetUserById(id).Result;
            if (existingUser == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.UpdateUser(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id) {
            var user = _SoundTrackRepository.GetUserById(id).Result;
            if (user == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.DeleteUser(user);
            return NoContent();
        }

        //Actualizar informacion del perfil del usuario
        [HttpPatch("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateProfileRequest request)
        {
            var user = await _SoundTrackRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Actualizar solo los que se modifico
            if (!string.IsNullOrEmpty(request.Username))
            {
                user.UserName = request.Username;
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }
            if (request.Bio != null) // Permitir string vacio
            {
                user.Bio = request.Bio;
            }
            if (!string.IsNullOrEmpty(request.ProfilePictureUrl))
            {
                user.ProfilePictureUrl = request.ProfilePictureUrl;
            }

            _SoundTrackRepository.UpdateUser(user);

            return Ok(new
            {
                message = "Profile updated successfully",
                user = new
                {
                    id = user.Id,
                    username = user.UserName,
                    email = user.Email,
                    bio = user.Bio,
                    profilePictureUrl = user.ProfilePictureUrl
                }
            });
        }

        // Actualizar favoritos del usuario
        [HttpPut("{id}/favorites")]
        public async Task<IActionResult> UpdateFavorites(string id, [FromBody] UpdateFavoritesRequest request)
        {
            var user = await _SoundTrackRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Validar que no se pasen mas de 3 IDs
            if (request.FavoriteArtistIds != null)
            {
                var artistIds = request.FavoriteArtistIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (artistIds.Length > 3)
                {
                    return BadRequest("Maximum 3 favorite artists allowed");
                }
                user.FavoriteArtistIds = request.FavoriteArtistIds;
            }

            if (request.FavoriteAlbumIds != null)
            {
                var albumIds = request.FavoriteAlbumIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (albumIds.Length > 3)
                {
                    return BadRequest("Maximum 3 favorite albums allowed");
                }
                user.FavoriteAlbumIds = request.FavoriteAlbumIds;
            }

            if (request.FavoriteSongIds != null)
            {
                var songIds = request.FavoriteSongIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (songIds.Length > 3)
                {
                    return BadRequest("Maximum 3 favorite songs allowed");
                }
                user.FavoriteSongIds = request.FavoriteSongIds;
            }

            _SoundTrackRepository.UpdateUser(user);

            return Ok(new
            {
                message = "Favorites updated successfully",
                favorites = new
                {
                    artists = user.FavoriteArtistIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
                    albums = user.FavoriteAlbumIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
                    songs = user.FavoriteSongIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()
                }
            });
        }

        // Obtener favoritos del usuario
        [HttpGet("{id}/favorites")]
        public async Task<IActionResult> GetFavorites(string id)
        {
            var user = await _SoundTrackRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                userId = user.Id,
                favorites = new
                {
                    artists = user.FavoriteArtistIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
                    albums = user.FavoriteAlbumIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
                    songs = user.FavoriteSongIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()
                }
            });
        }

        // Obtener perfil completo del usuario con estadísticas
        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetUserProfile(string id)
        {
            var user = await _SoundTrackRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Obtener cuantos revies swe han hecho
            var reviewCount = user.Reviews?.Count ?? 0;
            var followedArtistsCount = user.FollowedArtists?.Count ?? 0;

            return Ok(new
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email,
                bio = user.Bio,
                profilePictureUrl = user.ProfilePictureUrl,
                birthDay = user.BirthDay,
                createDate = user.CreateDate,
                statistics = new
                {
                    totalReviews = reviewCount,
                    followedArtists = followedArtistsCount,
                    followers = user.Followers?.Count ?? 0,
                    following = user.Following?.Count ?? 0
                },
                favorites = new
                {
                    artists = user.FavoriteArtistIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
                    albums = user.FavoriteAlbumIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>(),
                    songs = user.FavoriteSongIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()
                }
            });
        }
    }

    // DTOs
    public class UpdateProfileRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

    public class UpdateFavoritesRequest
    {
        public string? FavoriteArtistIds { get; set; }
        public string? FavoriteAlbumIds { get; set; }
        public string? FavoriteSongIds { get; set; }
    }
}