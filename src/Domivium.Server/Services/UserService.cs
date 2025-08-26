using Domivium.Server.AppServices;
using Domivium.Server.Utilities;
using Domivium.Shared.Common;
using Domivium.Shared.Request;
using Domivium.Shared.Response;
using Domivium.Shared.Services;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.AspNetCore.Authorization;

namespace Domivium.Server.Services;

[Authorize]
public class UserService(IUserAppService userAppService) : ServiceBase<IUserService>, IUserService
{
    [AllowAnonymous]
    public async UnaryResult<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var token = await userAppService.Register(request.UserId);
        return new RegisterResponse
        {
            StatusCode = StatusCode.Success,
            Token = token.ToDto(),
        };
    }

    [AllowAnonymous]
    public async UnaryResult<LoginResponse> LoginAsync(LoginRequest request)
    {
        var token = await userAppService.Login(request.UserId);
        return new LoginResponse
        {
            StatusCode = StatusCode.Success,
            Token = token.ToDto()
        };
    }

    [AllowAnonymous]
    public async UnaryResult<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var token = await userAppService.RefreshTokenAsync(request.RefreshToken);
        return new RefreshTokenResponse
        {
            StatusCode = StatusCode.Success,
            Token = token.ToDto()
        };
    }
}