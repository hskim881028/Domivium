using Domivium.Shared.Request;
using Domivium.Shared.Response;
using MagicOnion;

namespace Domivium.Shared.Services
{
    public interface ICharacterService : IService<ICharacterService>
    {
        public UnaryResult<GetCharactersResponse> GetCharactersAsync(GetCharactersRequest request);
    }
}