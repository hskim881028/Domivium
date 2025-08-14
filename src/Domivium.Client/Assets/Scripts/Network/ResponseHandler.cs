using Domivium.Client.Core.Utility;
using Domivium.Shared.Common;
using Domivium.Shared.Response;

namespace Domivium.Client.Network
{
    public class ResponseHandler : IResponseHandler
    {
        public bool HandleResponse(IResponse response)
        {
            if (response.StatusCode == StatusCode.Success) return true;

            ZLog.StatusCodeException(response.StatusCode);
            return false;
        }
    }
}