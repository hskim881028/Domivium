using System;
using MessagePack;

namespace Domivium.Shared.Response
{
    [MessagePackObject]
    public class LoginResponse : IResponse
    {
        [Key(0)] public ushort StatusCode { get; set; }
        [Key(1)] public string Token { get; set; }
        [Key(2)] public string UserId { get; set; }
        [Key(3)] public string Username { get; set; }
        [Key(4)] public bool IsNewUser { get; set; }
        [Key(5)] public DateTime LastLoginAt { get; set; }
    }
}