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
        // 실제로는 클라이언트에서 username을 받아야 하나, 기존 인터페이스 유지 위해 샘플 값 사용
        var response = await loginUseCase.LoginOrCreateAsync("tester");
        return response;
    }
}