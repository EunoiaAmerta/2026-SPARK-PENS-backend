using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
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
    /// Admin login with username/password
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        // First, check if this is default admin credentials BEFORE searching database
        // This allows auto-creation or password reset on first login
        if (loginDto.Username.ToLower() == "admin" && loginDto.Password == "admin")
        {
            // Check if admin user already exists
            var existingAdmin = await _context.Users
                .FirstOrDefaultAsync(u => u.Role == "Admin");

            if (existingAdmin != null)
            {
                // Admin exists, reset password to ensure it matches "admin"
                // This handles cases where password hash was corrupted or different
                existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin", 12);
                existingAdmin.HasPassword = true;
                await _context.SaveChangesAsync();

                var jwtToken = GenerateJwtToken(existingAdmin);
                return Ok(new AuthResponseDto
                {
                    Token = jwtToken,
                    User = new UserDto
                    {
                        Id = existingAdmin.Id,
                        Email = existingAdmin.Email,
                        Name = existingAdmin.Name,
                        Role = existingAdmin.Role
                    }
                });
            }

            // Create default admin user
            var newUser = new User
            {
                Email = "admin@sparkpens.com",
                Name = "Admin",
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin", 12),
                HasPassword = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var adminToken = GenerateJwtToken(newUser);
            return Ok(new AuthResponseDto
            {
                Token = adminToken,
                User = new UserDto
                {
                    Id = newUser.Id,
                    Email = newUser.Email,
                    Name = newUser.Name,
                    Role = newUser.Role
                }
            });
        }

        // Find user by email/username
        var dbUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Username || u.Name == loginDto.Username);
        
        // If user not found
        if (dbUser == null)
        {
            return Unauthorized(new { message = "User not found" });
        }
        
        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, dbUser.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var userToken = GenerateJwtToken(dbUser);
        
        return Ok(new AuthResponseDto
        {
            Token = userToken,
            User = new UserDto
            {
                Id = dbUser.Id,
                Email = dbUser.Email,
                Name = dbUser.Name,
                Role = dbUser.Role
            }
        });
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

            // Check if email is verified by Google
            var emailVerified = googleUser.EmailVerified;
            if (!emailVerified)
            {
                return BadRequest(new { message = "Email must be verified by Google" });
            }

            // FIRST: Check if user exists by Email (for account linking)
            var existingUserByEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == googleUser.Email.ToLower());

            User user;
            bool needsPasswordSetup = false;

            if (existingUserByEmail != null)
            {
                // User exists with this email - link GoogleId if not already linked
                if (string.IsNullOrEmpty(existingUserByEmail.GoogleId))
                {
                    existingUserByEmail.GoogleId = googleUser.Subject;
                }
                
                // Update name in case it changed
                existingUserByEmail.Name = googleUser.Name;
                
                // Check if user needs to set password
                needsPasswordSetup = !existingUserByEmail.HasPassword;
                
                await _context.SaveChangesAsync();
                user = existingUserByEmail;
            }
            else
            {
                // Check if user exists by GoogleId
                var existingUserByGoogleId = await _context.Users
                    .FirstOrDefaultAsync(u => u.GoogleId == googleUser.Subject);

                if (existingUserByGoogleId != null)
                {
                    // Update email and name
                    existingUserByGoogleId.Email = googleUser.Email;
                    existingUserByGoogleId.Name = googleUser.Name;
                    needsPasswordSetup = !existingUserByGoogleId.HasPassword;
                    
                    await _context.SaveChangesAsync();
                    user = existingUserByGoogleId;
                }
                else
                {
                    // Create new user - no password set yet (needs password setup)
                    user = new User
                    {
                        Email = googleUser.Email,
                        Name = googleUser.Name,
                        Role = "User",
                        GoogleId = googleUser.Subject,
                        HasPassword = false, // Will need to set password later
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    needsPasswordSetup = true;
                }
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
                },
                NeedsPasswordSetup = needsPasswordSetup
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Google login failed", error = ex.Message });
        }
    }

    /// <summary>
    /// Set password for user who logged in via Google (first time)
    /// </summary>
    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto setPasswordDto)
    {
        var user = await _context.Users.FindAsync(setPasswordDto.UserId);
        
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Hash and set the password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(setPasswordDto.NewPassword, 12);
        user.HasPassword = true;
        
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password set successfully" });
    }

    /// <summary>
    /// Request password reset - sends reset link via response
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == forgotPasswordDto.Email.ToLower());

        // If user not found - return error so frontend can show warning
        if (user == null)
        {
            return NotFound(new { message = "Email belum terdaftar di sistem kami." });
        }

        // Check if user has a password (Google-only users can't reset password this way)
        if (!user.HasPassword)
        {
            return BadRequest(new { message = "Akun ini menggunakan login Google. Silakan login dengan Google." });
        }

        // Generate reset token
        var resetToken = GenerateSecureToken();
        user.ResetToken = resetToken;
        user.ResetExpiry = DateTime.UtcNow.AddMinutes(30); // 30 minutes expiry
        
        await _context.SaveChangesAsync();

        // Use provided frontendUrl if available, otherwise detect from request
        string frontendUrl;
        if (!string.IsNullOrEmpty(forgotPasswordDto.FrontendUrl))
        {
            // Use the frontend URL provided by the client (e.g., localhost:5173 or https://spark-pens.vercel.app)
            frontendUrl = forgotPasswordDto.FrontendUrl.TrimEnd('/');
        }
        else
        {
            // Fallback to detection from request headers
            frontendUrl = DetectFrontendUrl(Request);
        }
        
        var resetLink = $"{frontendUrl}/reset-password?token={resetToken}&email={user.Email}";

        // Return the reset link (in production, this would be sent via email)
        return Ok(new { 
            message = "Link reset berhasil dibuat",
            resetLink = resetLink
        });
    }

    /// <summary>
    /// Detect frontend URL based on request origin (supports both localhost and Vercel)
    /// </summary>
    private string DetectFrontendUrl(HttpRequest request)
    {
        // Check the Origin header to determine the frontend URL
        var origin = request.Headers["Origin"].ToString();
        
        if (!string.IsNullOrEmpty(origin))
        {
            // If request comes from localhost, use localhost
            if (origin.Contains("localhost"))
            {
                return "http://localhost:5173";
            }
            // If request comes from Vercel, use Vercel URL
            if (origin.Contains("vercel.app") || origin.Contains("spark-pens"))
            {
                return "https://spark-pens.vercel.app";
            }
        }

        // Fallback: Check Referer header
        var referer = request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer))
        {
            if (referer.Contains("localhost"))
            {
                return "http://localhost:5173";
            }
            if (referer.Contains("vercel.app") || referer.Contains("spark-pens"))
            {
                return "https://spark-pens.vercel.app";
            }
        }

        // Default: Use config or fallback to Vercel
        return _configuration["FrontendUrl"] ?? "https://spark-pens.vercel.app";
    }

    /// <summary>
    /// Reset password using token
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        // Debug: Log the received token
        Console.WriteLine($"[ResetPassword] Received token: '{resetPasswordDto.Token}'");
        
        // Find user by token (case-sensitive exact match, trim whitespace)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.ResetToken != null && u.ResetToken == resetPasswordDto.Token.Trim());
        
        if (user == null)
        {
            // Debug: Check what tokens exist in database
            var allUsersWithTokens = await _context.Users
                .Where(u => u.ResetToken != null)
                .Select(u => new { u.Email, u.ResetToken, u.ResetExpiry })
                .ToListAsync();
            
            Console.WriteLine($"[ResetPassword] Users with tokens in DB: {allUsersWithTokens.Count}");
            foreach (var u in allUsersWithTokens)
            {
                Console.WriteLine($"[ResetPassword] - Email: {u.Email}, Token: '{u.ResetToken}', Expiry: {u.ResetExpiry}");
            }
            
            return BadRequest(new { message = "Invalid reset token" });
        }

        // Check if token is expired
        if (user.ResetExpiry == null || user.ResetExpiry < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Reset token has expired" });
        }

        // Hash and set new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword, 12);
        user.HasPassword = true;
        
        // Clear reset token
        user.ResetToken = null;
        user.ResetExpiry = null;
        
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully" });
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

    private string GenerateSecureToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        
        // Use URL-safe Base64 encoding (replace + with -, / with _, and remove = padding)
        var base64 = Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
        
        return base64;
    }

    private async Task<GoogleUserInfo?> ValidateGoogleToken(string credential)
    {
        try
        {
            // Decode the JWT token from Google (client-side validation already done by Google)
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(credential);

            // Note: Audience validation is skipped because:
            // 1. Frontend already validates with Google
            // 2. We don't have Google:ClientId configured in appsettings.json
            
            // Optional: Validate audience if configured
            // var clientId = _configuration["Google:ClientId"];
            // var tokenAudience = jwtToken.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
            // if (!string.IsNullOrEmpty(clientId) && tokenAudience != clientId)
            // {
            //     return null;
            // }

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
                       jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "Google User",
                EmailVerified = jwtToken.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value == "true"
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
        public bool EmailVerified { get; set; } = false;
    }
}

