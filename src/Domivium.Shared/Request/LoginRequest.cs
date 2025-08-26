using System;
using MessagePack;

namespace Domivium.Shared.Request
{
    [MessagePackObject]
    public class LoginRequest
    {
        [Key(0)] public Guid UserId { get; set; }
    }
}