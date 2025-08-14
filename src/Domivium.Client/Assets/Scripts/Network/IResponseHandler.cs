using Domivium.Shared.Response;

namespace Domivium.Client.Network
{
    public interface IResponseHandler
    {
        public bool HandleResponse(IResponse response);
    }
}