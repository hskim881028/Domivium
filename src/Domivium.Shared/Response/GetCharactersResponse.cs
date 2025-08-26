using MessagePack;

namespace Domivium.Shared.Response
{
    [MessagePackObject]
    public class GetCharactersResponse : IResponse
    {
        [Key(0)] public int StatusCode { get; set; }
        [Key(1)] public string Message { get; set; }
    }
}