using System;
using MessagePack;

namespace Domivium.Shared.DataTransferObject
{
    [MessagePackObject]
    public class UserDto
    {
        [Key(0)] public Guid Id { get; set; }
    }
}