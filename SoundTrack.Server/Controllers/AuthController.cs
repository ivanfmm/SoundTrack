using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Data;
using SoundTrack.Server.Models;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Cambiar de IdentityUser a User
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly SoundTrackContext _context;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            SoundTrackContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ahora solo creas UN usuario (User ya ES un IdentityUser)
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                CreateDate = DateTime.UtcNow,
                BirthDay = model.BirthDay.HasValue
                    ? DateTime.SpecifyKind(model.BirthDay.Value, DateTimeKind.Utc)
                    : DateTime.UtcNow.AddYears(-18),
                ProfilePictureUrl = null,
                Bio = ""
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            // Login automático
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok(new
            {
                message = "User registered successfully",
                userId = user.Id,
                username = user.UserName
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                return Ok(new
                {
                    message = "Login successful",
                    userId = user.Id,
                    username = user.UserName
                });
            }

            if (result.IsLockedOut)
            {
                return BadRequest(new { message = "Account locked out" });
            }

            return Unauthorized(new { message = "Invalid username or password" });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout successful" });
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Cargar las relaciones si es necesario
            await _context.Entry(user)
                .Collection(u => u.Followers)
                .LoadAsync();
            await _context.Entry(user)
                .Collection(u => u.Following)
                .LoadAsync();

            return Ok(new
            {
                userId = user.Id,
                username = user.UserName,
                email = user.Email,
                profilePictureUrl = user.ProfilePictureUrl,
                birthDay = user.BirthDay,
                bio = user.Bio,
                followersCount = user.Followers?.Count ?? 0,
                followingCount = user.Following?.Count ?? 0
            });
        }
    }

    // DTOs
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? BirthDay { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}