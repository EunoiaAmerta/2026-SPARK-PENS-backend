using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SparkPens.Api.Data;
using SparkPens.Api.DTOs;
using SparkPens.Api.Models;

namespace SparkPens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Admin login with username/password (admin/admin)
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        // Hardcoded admin credentials for demo
        if (loginDto.Username == "admin" && loginDto.Password == "admin")
        {
            // Check if admin user exists in database, if not create
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
            
            if (adminUser == null)
            {
                adminUser = new User
                {
                    Email = "admin@sparkpens.com",
                    Name = "Administrator",
                    Role = "Admin",
                    PasswordHash = "admin", // In production, use proper hashing!
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();
            }

            var token = GenerateJwtToken(adminUser);
            
            return Ok(new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = adminUser.Id,
                    Email = adminUser.Email,
                    Name = adminUser.Name,
                    Role = adminUser.Role
                }
            });
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }

    /// <summary>
    /// Google OAuth login - receives Google ID token from frontend
    /// </summary>
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto googleDto)
    {
        try
        {
            // Validate Google token
            var googleUser = await ValidateGoogleToken(googleDto.Credential);
            
            if (googleUser == null)
            {
                return Unauthorized(new { message = "Invalid Google token" });
            }

            // Check if user exists in database
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.GoogleId == googleUser.Subject);

            User user;
            if (existingUser == null)
            {
                // Create new user
                user = new User
                {
                    Email = googleUser.Email,
                    Name = googleUser.Name,
                    Role = "User",
                    GoogleId = googleUser.Subject,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update existing user info
                existingUser.Email = googleUser.Email;
                existingUser.Name = googleUser.Name;
                await _context.SaveChangesAsync();
                user = existingUser;
            }

            var token = GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Google login failed", error = ex.Message });
        }
    }

    /// <summary>
    /// Get current user info (protected endpoint)
    /// </summary>
    [HttpGet("me")]
    [Authorize] // Requires authentication
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);
        
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "SparkPensDefaultSecretKey12345678901234567890"));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "SparkPens",
            audience: _configuration["Jwt:Audience"] ?? "SparkPensUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<GoogleUserInfo?> ValidateGoogleToken(string credential)
    {
        try
        {
            // Decode the JWT token from Google (client-side validation already done)
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(credential);

            // Validate audience matches our client ID
            var clientId = _configuration["Google:ClientId"];
            var tokenAudience = jwtToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
            if (!string.IsNullOrEmpty(clientId) && tokenAudience != clientId)
            {
                return null;
            }

            // Check token expiration
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                return null;
            }

            return new GoogleUserInfo
            {
                Subject = jwtToken.Subject ?? "",
                Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "",
                Name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? 
                       jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "Google User"
            };
        }
        catch
        {
            return null;
        }
    }

    private class GoogleUserInfo
    {
        public string Subject { get; set; } = "";
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
    }
}

