using System;
using MessagePack;

namespace Domivium.Shared.Request
{
    [MessagePackObject]
    public class RefreshTokenRequest
    {
        [Key(0)] public string RefreshToken { get; set; }
    }
}