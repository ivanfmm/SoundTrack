using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Services;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        // Inyección de dependencia
        private readonly ISoundTrackRepository _repository;

        public HomeController(ISoundTrackRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Home/top-artists?count=5
        [HttpGet("top-artists")]
        public async Task<IActionResult> GetTopArtists([FromQuery] int count = 5)
        {
            try
            {
                var artists = await _repository.GetTopRatedArtists(count);

                var result = new List<object>();
                foreach (var artist in artists)
                {
                    var avgScore = await _repository.GetAverageScore(artist.Id, "artist");
                    var reviewCount = await _repository.GetReviewCountByProfile(artist.Id, "artist");

                    result.Add(new
                    {
                        id = artist.Id,
                        name = artist.Name,
                        imageUrl = artist.ImageUrl,
                        averageScore = avgScore,
                        reviewCount = reviewCount
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener top artistas", error = ex.Message });
            }
        }

        // GET: api/Home/top-albums?count=5
        [HttpGet("top-albums")]
        public async Task<IActionResult> GetTopAlbums([FromQuery] int count = 5)
        {
            try
            {
                var albums = await _repository.GetTopRatedAlbums(count);

                var result = new List<object>();
                foreach (var album in albums)
                {
                    var avgScore = await _repository.GetAverageScore(album.Id, "album");
                    var reviewCount = await _repository.GetReviewCountByProfile(album.Id, "album");

                    result.Add(new
                    {
                        id = album.Id,
                        name = album.Name,
                        imageUrl = album.ImageUrl,
                        averageScore = avgScore,
                        reviewCount = reviewCount
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener top álbumes", error = ex.Message });
            }
        }

        // GET: api/Home/top-songs?count=5
        [HttpGet("top-songs")]
        public async Task<IActionResult> GetTopSongs([FromQuery] int count = 5)
        {
            try
            {
                var songs = await _repository.GetTopRatedSongs(count);

                var result = new List<object>();
                foreach (var song in songs)
                {
                    var avgScore = await _repository.GetAverageScore(song.Id, "song");
                    var reviewCount = await _repository.GetReviewCountByProfile(song.Id, "song");

                    result.Add(new
                    {
                        id = song.Id,
                        name = song.Name,
                        imageUrl = song.ImageUrl,
                        averageScore = avgScore,
                        reviewCount = reviewCount
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener top canciones", error = ex.Message });
            }
        }

        // GET: api/Home/top-reviews?count=5
        [HttpGet("top-reviews")]
        public async Task<IActionResult> GetTopReviews([FromQuery] int count = 5)
        {
            try
            {
                var reviews = await _repository.GetTopLikedReviews(count);

                var result = reviews.Select(r => new
                {
                    id = r.Id,
                    title = r.Title,
                    body = r.Content,
                    score = r.score,
                    likes = r.Likes,
                    dislikes = r.Dislikes,
                    createdAt = r.CreatedAt,
                    user = new
                    {
                        id = r.User.Id,
                        username = r.User.UserName,
                        imageUrl = r.User.ProfilePictureUrl
                    },
                    profile = r.SongProfileId != null ? new
                    {
                        type = "song",
                        id = r.SongProfile.Id,
                        name = r.SongProfile.Name,
                        imageUrl = r.SongProfile.ImageUrl
                    } : r.ArtistProfileId != null ? new
                    {
                        type = "artist",
                        id = r.ArtistProfile.Id,
                        name = r.ArtistProfile.Name,
                        imageUrl = r.ArtistProfile.ImageUrl
                    } : new
                    {
                        type = "album",
                        id = r.AlbumProfile.Id,
                        name = r.AlbumProfile.Name,
                        imageUrl = r.AlbumProfile.ImageUrl
                    }
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener top reviews", error = ex.Message });
            }
        }
    }
}