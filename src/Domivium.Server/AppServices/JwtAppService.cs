using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domivium.Server.Models;
using Domivium.Server.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Domivium.Server.AppServices;

public class JwtAppService : IJwtAppService
{
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly JwtSettings _settings;
    private readonly TokenValidationParameters _validationParameters;
    private readonly SymmetricSecurityKey _key;

    public JwtAppService(JwtSettings settings, TokenValidationParameters validationParameters)
    {
        _settings = settings;
        _validationParameters = validationParameters;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
    }

    public string HashToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(bytes);
    }
    
    public ClaimsPrincipal? Validate(string accessToken)
    {
        try
        {
            return _handler.ValidateToken(accessToken, _validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }

    public TokenModel CreateToken(Guid userId)
    {
        var accessToken = GenerateAccessToken(userId);
        var refreshToken = GenerateRefreshToken();
        return new TokenModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenLifetimeSeconds = _settings.AccessTokenLifetimeSeconds,
            RefreshTokenLifetimeSeconds = _settings.RefreshTokenLifetimeSeconds
        };
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateAccessToken(Guid userId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var descriptor = new SecurityTokenDescriptor
        {
            Audience = _settings.Audience,
            Expires = DateTime.UtcNow.AddSeconds(_settings.AccessTokenLifetimeSeconds),
            Issuer = _settings.Issuer,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature),
            Subject = new ClaimsIdentity(claims)
        };

        var token = _handler.CreateToken(descriptor);
        return _handler.WriteToken(token);
    }
}