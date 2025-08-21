using Domivium.Server.Data;
using Domivium.Server.Models;
using Domivium.Server.Utilities;
using Domivium.Shared.Response;
using Microsoft.EntityFrameworkCore;

namespace Domivium.Server.UseCases.Login;

public class LoginUseCase : ILoginUseCase
{
    private readonly AppDbContext _db;
    public LoginUseCase(AppDbContext db) => _db = db;

    public async Task<LoginResponse> LoginOrCreateAsync(string username)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Name == username);

        if (user is not null)
        {
            return new LoginResponse
            {
                StatusCode = 0,
                Token = Guid.NewGuid().ToString(), // TODO: 실제 JWT 토큰으로 변경 필요
                User = user.ToDto(),
                IsNewUser = false,
            };
        }

        user = new User { Name = username };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new LoginResponse
        {
            StatusCode = 0,
            Token = Guid.NewGuid().ToString(), // TODO: 실제 JWT 토큰으로 변경 필요
            User = user.ToDto(),
            IsNewUser = true,
        };
    }
}