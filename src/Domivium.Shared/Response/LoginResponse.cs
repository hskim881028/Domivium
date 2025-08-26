using Domivium.Shared.DataTransferObject;
using MessagePack;

namespace Domivium.Shared.Response
{
    [MessagePackObject]
    public class LoginResponse : IResponse
    {
        [Key(0)] public int StatusCode { get; set; }
        [Key(1)] public TokenDto Token { get; set; }
    }
}