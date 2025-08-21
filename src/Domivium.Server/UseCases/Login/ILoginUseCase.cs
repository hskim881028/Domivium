using Domivium.Shared.Response;

namespace Domivium.Server.UseCases.Login;

public interface ILoginUseCase
{
    Task<LoginResponse> LoginOrCreateAsync(string username);
}