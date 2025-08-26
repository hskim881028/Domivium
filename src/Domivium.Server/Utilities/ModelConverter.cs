using Domivium.Server.Models;
using Domivium.Shared.DataTransferObject;

namespace Domivium.Server.Utilities;

public static class ModelConverter
{
    public static UserDto ToDto(this UserModel userModel)
    {
        return new UserDto
        {
            Id = userModel.Id,
        };
    }

    public static TokenDto ToDto(this TokenModel token)
    {
        return new TokenDto
        {
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            AccessTokenLifetimeSeconds = token.AccessTokenLifetimeSeconds,
            RefreshTokenLifetimeSeconds = token.RefreshTokenLifetimeSeconds,
        };
    }
}