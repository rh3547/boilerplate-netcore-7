namespace Nukleus.Application.Authentication
{
    public record AuthenticationResult(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Token);

    public record TokenDTO
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }

    public record ForgotPasswordDTO
    {
        public string Email { get; set; } = null!;
    }

    public record ResetPasswordDTO
    {
        public string EmailToken { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AuthenticateDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}