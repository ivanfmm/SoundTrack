using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using SoundTrack.Server.Data;
using SoundTrack.Server.Services;
using System.Configuration;
namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("[ArtistProfile]")]
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
    }
}
