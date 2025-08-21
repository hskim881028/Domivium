using Domivium.Server.Models;
using Domivium.Shared.DataTransferObject;

namespace Domivium.Server.Utilities;

public static class ModelConverter
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
        };
    }
}