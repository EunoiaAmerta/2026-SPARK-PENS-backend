namespace SparkPens.Api.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
    public bool NeedsPasswordSetup { get; set; } = false;
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

// DTO for setting password (after Google login)
public class SetPasswordDto
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}

// DTO for forgot password request
public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string? FrontendUrl { get; set; }
}

// DTO for reset password
public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

