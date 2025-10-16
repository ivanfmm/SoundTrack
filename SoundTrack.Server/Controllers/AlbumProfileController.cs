using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Services;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("[AlbumProfile]")]
    public class AlbumProfileController : Controller
    {
        private readonly ISoundTrackRepository _SoundTrackRepository;
        public AlbumProfileController(ISoundTrackRepository SoundTrackRepository)
        {
            _SoundTrackRepository = SoundTrackRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAlbumProfile()
        {
            var albumProfile = await _SoundTrackRepository.GetAllAlbumProfile();
            return Ok(albumProfile);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlbumProfileById(string id)
        {
            var albumProfile = await _SoundTrackRepository.GetAlbumProfileById(id);
            if (albumProfile == null)
            {
                return NotFound();
            }
            return Ok(albumProfile);
        }
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetAlbumProfileByName(string name)
        {
            var albumProfile = await _SoundTrackRepository.GetAlbumProfileByName(name);
            if (albumProfile == null)
            {
                return NotFound();
            }
            return Ok(albumProfile);
        }
        [HttpPost]
        public IActionResult AddAlbumProfile([FromBody] Models.AlbumProfile albumProfile)
        {
            _SoundTrackRepository.addAlbumProfile(albumProfile);
            return CreatedAtAction(nameof(GetAlbumProfileById), new { id = albumProfile.Id }, albumProfile);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateAlbumProfile(string id, [FromBody] Models.AlbumProfile albumProfile)
        {
            if (id != albumProfile.Id)
            {
                return BadRequest();
            }
            var existingAlbumProfile = _SoundTrackRepository.GetAlbumProfileById(id).Result;
            if (existingAlbumProfile == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.updateAlbumProfile(albumProfile);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteAlbumProfile(string id)
        {
            var existingAlbumProfile = _SoundTrackRepository.GetAlbumProfileById(id).Result;
            if (existingAlbumProfile == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.deleteAlbumProfile(existingAlbumProfile);
            return NoContent();
        }
    }
}
