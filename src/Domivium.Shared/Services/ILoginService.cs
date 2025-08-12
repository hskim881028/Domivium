using Domivium.Shared.Response;
using MagicOnion;

namespace Domivium.Shared.Services
{
    public interface ILoginService : IService<ILoginService>
    {
        public UnaryResult<LoginResponse> Login();
    }
}