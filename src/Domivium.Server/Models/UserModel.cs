namespace Domivium.Server.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string RefreshTokenHash { get; set; }
    public DateTime RefreshTokenExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}