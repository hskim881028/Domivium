using Domivium.Server.Models;

namespace Domivium.Server.AppServices;

public interface IUserAppService
{
    public Task<TokenModel> Register(Guid userId);
    public Task<TokenModel> Login(Guid userId);
    public Task<TokenModel> RefreshTokenAsync(string refreshToken);
}