using Domivium.Shared.DataTransferObject;
using MessagePack;

namespace Domivium.Shared.Response
{
    [MessagePackObject]
    public class LoginResponse : IResponse
    {
        [Key(0)] public ushort StatusCode { get; set; }
        [Key(1)] public string Token { get; set; }
        [Key(2)] public UserDto User { get; set; }
        [Key(3)] public bool IsNewUser { get; set; }
    }
}