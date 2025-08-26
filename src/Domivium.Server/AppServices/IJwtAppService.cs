using System.Security.Claims;
using Domivium.Server.Models;

namespace Domivium.Server.AppServices;

public interface IJwtAppService
{
    public string HashToken(string refreshToken);
    public ClaimsPrincipal? Validate(string accessToken);
    public TokenModel CreateToken(Guid userId);
}