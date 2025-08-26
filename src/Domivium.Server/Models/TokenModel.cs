namespace Domivium.Server.Models;

public class TokenModel
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int AccessTokenLifetimeSeconds { get; set; }
    public int RefreshTokenLifetimeSeconds { get; set; }
}