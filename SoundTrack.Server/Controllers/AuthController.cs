using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoundTrack.Server.Data;
using SoundTrack.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace SoundTrack.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly SoundTrackContext _context;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            SoundTrackContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Crear usuario en Identity
            var identityUser = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            // 2. Crear registro en tabla Users (tu modelo personalizado)
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                IdentityUserId = identityUser.Id,
                CreateDate = DateTime.UtcNow,
                BirthDay = model.BirthDay ?? DateTime.UtcNow.AddYears(-18),
                ProfilePictureUrl = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3. Login automático
            await _signInManager.SignInAsync(identityUser, isPersistent: false);

            return Ok(new
            {
                message = "User registered successfully",
                userId = user.Id,
                identityUserId = identityUser.Id,
                username = user.Username
            });
        }

        // POST: api/Auth/login
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
                var identityUser = await _userManager.FindByNameAsync(model.Username);
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.IdentityUserId == identityUser.Id);

                return Ok(new
                {
                    message = "Login successful",
                    userId = user?.Id,
                    identityUserId = identityUser.Id,
                    username = identityUser.UserName
                });
            }

            if (result.IsLockedOut)
            {
                return BadRequest(new { message = "Account locked out" });
            }

            return Unauthorized(new { message = "Invalid username or password" });
        }

        // POST: api/Auth/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout successful" });
        }

        // GET: api/Auth/current
        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _context.Users
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUser.Id);

            return Ok(new
            {
                userId = user?.Id,
                identityUserId = identityUser.Id,
                username = identityUser.UserName,
                email = identityUser.Email,
                profilePictureUrl = user?.ProfilePictureUrl,
                birthDay = user?.BirthDay,
                followersCount = user?.Followers?.Count ?? 0,
                followingCount = user?.Following?.Count ?? 0
            });
        }

        // POST: api/Auth/migrate-existing-user
        // Migrar usuarios que ya existen en la tabla Users
        [HttpPost("migrate-existing-user")]
        public async Task<IActionResult> MigrateExistingUser([FromBody] MigrateUserDto model)
        {
            // 1. Buscar usuario en tabla Users antigua
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!string.IsNullOrEmpty(existingUser.IdentityUserId))
            {
                return BadRequest(new { message = "User already migrated" });
            }

            // 2. Crear en Identity
            var identityUser = new IdentityUser
            {
                UserName = existingUser.Username,
                Email = existingUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            // 3. Vincular
            existingUser.IdentityUserId = identityUser.Id;
            await _context.SaveChangesAsync();

            // 4. Login automático
            await _signInManager.SignInAsync(identityUser, false);

            return Ok(new
            {
                message = "User migrated successfully",
                userId = existingUser.Id,
                identityUserId = identityUser.Id
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

    public class MigrateUserDto
    {
        public string Username { get; set; }
        public string NewPassword { get; set; }
    }
}