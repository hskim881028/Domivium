namespace Domivium.Server.Settings;

public class JwtSettings
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int ClockSkewSeconds { get; init; } = 60;
    public int AccessTokenLifetimeSeconds { get; init; } = 900;
    public int RefreshTokenLifetimeSeconds { get; init; } = 86400;
}