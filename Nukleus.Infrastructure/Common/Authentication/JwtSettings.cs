
namespace Nukleus.Infrastructure.Common.Authentication
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        // Signing Key (Secret)
        public string Secret { get; init; } = null!;

        // Basically another secret/signing key, but completely not used in the actual 
        public string Pepper { get; init; } = null!;

        // Offset from IDateTimeProvder 'Now'
        public TimeSpan RefreshTokenExpiryTimeSpanOffset { get; init; }

        // Offset from IDateTimeProvder 'Now'
        public TimeSpan AccessTokenExpiryTimeSpanOffset { get; init; }
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
    }
}