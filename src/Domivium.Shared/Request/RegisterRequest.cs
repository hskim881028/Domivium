using System;
using MessagePack;

namespace Domivium.Shared.Request
{
    [MessagePackObject]
    public class RegisterRequest
    {
        [Key(0)] public Guid UserId { get; set; }
    }
}