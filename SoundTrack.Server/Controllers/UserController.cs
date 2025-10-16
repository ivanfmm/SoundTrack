using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using SoundTrack.Server.Data;
using SoundTrack.Server.Services;
using System.Configuration;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("[User]")]
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
        public async Task<IActionResult> GetUserById(int id)
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
        public IActionResult UpdateUser(int id, [FromBody] Models.User user)
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
        public IActionResult DeleteUser(int id) {
            var user = _SoundTrackRepository.GetUserById(id).Result;
            if (user == null)
            {
                return NotFound();
            }
            _SoundTrackRepository.DeleteUser(user);
            return NoContent();
        }
    }
}

