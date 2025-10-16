using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Services;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("[SongProfile]")]
    public class SongProfileController : Controller
    {
        private readonly ISoundTrackRepository _SoundTrackRepository;
        public SongProfileController(ISoundTrackRepository SoundTrackRepository)
        {
            _SoundTrackRepository = SoundTrackRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongProfile()
        {
            var songProfiles = await _SoundTrackRepository.GetAllSongProfile();
            return Ok(songProfiles);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongProfileById(string id)
        {
            var songProfile = await _SoundTrackRepository.GetSongProfileById(id);
            if (songProfile == null)
            {
                return NotFound();
            }
            return Ok(songProfile);
        }
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetSongProfileByName(string name)
        {
            var songProfile = await _SoundTrackRepository.GetSongProfileByName(name);
            if (songProfile == null)
            {
                return NotFound();
            }
            return Ok(songProfile);
        }
        [HttpPost]
        public IActionResult AddSongProfile([FromBody] Models.SongProfile songProfile)
        {
            _SoundTrackRepository.addSongProfile(songProfile);
            return CreatedAtAction(nameof(GetSongProfileById), new { id = songProfile.Id }, songProfile);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateSongProfile(string id, [FromBody] Models.SongProfile songProfile)
        {
            if (id != songProfile.Id)
            {
                return BadRequest();
            }
            var existingSongProfile = _SoundTrackRepository.GetSongProfileById(id).Result;
            if (existingSongProfile == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.updateSongProfile(songProfile);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteSongProfile(string id)
        {
            var existingSongProfile = _SoundTrackRepository.GetSongProfileById(id).Result;
            if (existingSongProfile == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.deleteSongProfile(existingSongProfile);
            return NoContent();
        }
    }
}
