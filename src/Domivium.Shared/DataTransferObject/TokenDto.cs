using MessagePack;

namespace Domivium.Shared.DataTransferObject
{
    [MessagePackObject]
    public class TokenDto
    {
        [Key(0)] public string AccessToken { get; set; }

        [Key(1)] public string RefreshToken { get; set; }

        [Key(2)] public int AccessTokenLifetimeSeconds { get; set; }

        [Key(3)] public int RefreshTokenLifetimeSeconds { get; set; }
    }
}