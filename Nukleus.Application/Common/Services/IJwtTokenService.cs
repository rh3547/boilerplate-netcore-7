using Nukleus.Domain.Common;

namespace Nukleus.Application.Common.Services
{
    public interface IJwtTokenService
    {
        public string GenerateAccessToken(Guid userId);

        public string GenerateRefreshToken(Guid userId);

        public Task<bool> VerifyToken(string token, bool validateLifetime = true);
    }
}