using System.Globalization;
using Domivium.Shared.Common;
using Domivium.Shared.Request;
using Domivium.Shared.Response;
using Domivium.Shared.Services;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.AspNetCore.Authorization;

namespace Domivium.Server.Services;

[Authorize]
public class CharacterService : ServiceBase<ICharacterService>, ICharacterService
{
    public async UnaryResult<GetCharactersResponse> GetCharactersAsync(GetCharactersRequest request)
    {
        return new GetCharactersResponse
        {
            StatusCode = StatusCode.Success,
            Message = DateTime.Now.ToString(CultureInfo.InvariantCulture)
        };
    }
}