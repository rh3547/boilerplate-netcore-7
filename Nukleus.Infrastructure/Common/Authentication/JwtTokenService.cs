using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Nukleus.Application.Common.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nukleus.Domain.Entities;
using Nukleus.Application.UserModule;
using Nukleus.Application.Common.Validation;
using Microsoft.EntityFrameworkCore;

namespace Nukleus.Infrastructure.Common.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserRepository _usersRepository;
    private readonly ISession _session;

    // https://security.stackexchange.com/questions/3272/password-hashing-add-salt-pepper-or-is-salt-enough
    public JwtTokenService(IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtOptions, IUserRepository usersRepository, ISession session)
    {
        _dateTimeProvider = dateTimeProvider;
        _jwtSettings = jwtOptions.Value;
        _usersRepository = usersRepository;
        _session = session;
    }

    public string GenerateAccessToken(Guid userId)
    {
        return GenerateToken(userId, _jwtSettings.AccessTokenExpiryTimeSpanOffset);
    }

    public string GenerateRefreshToken(Guid userId)
    {
        return GenerateToken(userId, _jwtSettings.RefreshTokenExpiryTimeSpanOffset);
    }

    public string GenerateToken(Guid userId, TimeSpan expiryTimeSpanOffset)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256);

        Result<User?> UserResult = _usersRepository.Find(userId);

        if(UserResult.IsFaulted || UserResult.Value == null)
        {
            // TODO: Return error
            return "";
        }

        var claims = new[]
        {
            // https://auth0.com/docs/secure/tokens/json-web-tokens/json-web-token-claims

            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("username", UserResult.Value.Username),
            new Claim("firstName", UserResult.Value.FirstName),
            new Claim("lastName", UserResult.Value.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _dateTimeProvider.UtcNow.Add(expiryTimeSpanOffset),
            SigningCredentials = signingCredentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }

    // private Result<Guid> TryParseClaimToGuid(JwtRegisteredClaimNames guidClaim)
    // {
    //     try
    //     {
    //         if(guidClaim JwtRegisteredClaimNames.Sub)
    //         {
    //             return Error.GenericError("Null value while parsing guidClaim on JWT.");
    //         }

    //         return Guid.Parse(guidClaim.Value);
    //     }
    //     catch
    //     {
    //         return Error.UnknownError("Error while parsing guidClaim to GUID on JWT.");
    //     }
    // }

    public async Task<bool> VerifyToken(string token, bool validateLifetime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = default;
        try
        {
            jwtToken = tokenHandler.ReadJwtToken(token);
        }
        catch
        {
            return false;
        }
        jwtToken = tokenHandler.ReadJwtToken(token);
        Claim? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        if (userIdClaim == default)
        {
            return false;
        }

        // If we fail to get the guid claim, fail to verify...
        // Result<Guid> guidFromClaimResult = TryParseClaimToGuid(userIdClaim);
        // if(guidFromClaimResult.IsFaulted)
        // {
        //     return false;
        // }

        // If we fail to get the User from the DB (doesn't exist?), fail to verify...
        IQueryable<User> query = _usersRepository.QueryWithTracking().Where(x => x.Id == Guid.Parse(userIdClaim.Value)).Include(x => x.Account);
        Result<User> userResult = await _usersRepository.GetSingleAsync(query);
        if(userResult.IsFaulted)
        {
            return false;
        }

        _session.SetUser(userResult.Value);
        _session.SetAccount(userResult.Value.Account);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}