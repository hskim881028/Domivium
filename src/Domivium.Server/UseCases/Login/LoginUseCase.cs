namespace Domivium.Server.UseCases.Login;

public class LoginUseCase : ILoginUseCase
{
    public Task<int> Test()
    {
        return Task.FromResult(505050);
    }
}