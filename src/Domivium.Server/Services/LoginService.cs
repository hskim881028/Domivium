using Domivium.Server.UseCases.Login;
using Domivium.Shared.Response;
using Domivium.Shared.Services;
using MagicOnion;
using MagicOnion.Server;

namespace Domivium.Server.Services;

public class LoginService(ILoginUseCase loginUseCase) : ServiceBase<ILoginService>, ILoginService
{
    public async UnaryResult<LoginResponse> Login()
    {
        var num = await loginUseCase.Test();
        var response = new LoginResponse
        {
            StatusCode = 0,
            Token = Guid.NewGuid().ToString(),
            UserId = num.ToString(),
            Username = "tester",
            IsNewUser = true,
            LastLoginAt = DateTime.UtcNow
        };
        return response;
    }
}