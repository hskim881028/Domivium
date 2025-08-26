using Domivium.Shared.Request;
using Domivium.Shared.Response;
using MagicOnion;

namespace Domivium.Shared.Services
{
    public interface IUserService : IService<IUserService>
    {
        public UnaryResult<RegisterResponse> RegisterAsync(RegisterRequest request);
        public UnaryResult<LoginResponse> LoginAsync(LoginRequest request);
        public UnaryResult<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    }
}