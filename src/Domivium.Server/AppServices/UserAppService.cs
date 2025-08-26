using Domivium.Server.Data;
using Domivium.Server.Models;
using Domivium.Shared.Common;
using Domivium.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Domivium.Server.AppServices;

public class UserAppService : IUserAppService
{
    private static string TokenHashKey(string hash) => $"token:hash:{hash}";

    private readonly IDatabase _redis;
    private readonly AppDbContext _db;
    private readonly IPostCommitQueue _postCommit;
    private readonly IJwtAppService _jwtAppService;
    private readonly ILogger<UserAppService> _logger;

    public UserAppService(
        IConnectionMultiplexer redis,
        AppDbContext db,
        IPostCommitQueue postCommit,
        IJwtAppService jwtAppService,
        ILogger<UserAppService> logger)
    {
        _redis = redis.GetDatabase();
        _db = db;
        _postCommit = postCommit;
        _jwtAppService = jwtAppService;
        _logger = logger;
    }

    public async Task<TokenModel> Register(Guid userId)
    {
        var existedUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (existedUser != null)
        {
            throw new ServerExceptions(StatusCode.UserAlreadyExists, $"User already exists: {userId}");
        }

        var token = _jwtAppService.CreateToken(userId);
        var createAt = DateTime.UtcNow;
        var newHash = _jwtAppService.HashToken(token.RefreshToken);
        var refreshTokenExpiredAt = createAt.AddSeconds(token.RefreshTokenLifetimeSeconds);

        var user = new UserModel
        {
            Id = userId,
            RefreshTokenHash = newHash,
            RefreshTokenExpiredAt = refreshTokenExpiredAt,
            CreatedAt = createAt,
        };
        _db.Users.Add(user);

        var expiry = TimeSpan.FromSeconds(token.RefreshTokenLifetimeSeconds);
        _postCommit.Enqueue(async _ => await _redis.StringSetAsync(TokenHashKey(newHash), userId.ToString(), expiry));
        
        _logger.LogInformation("[Register] userId={UserId} newHash={New}", user.Id, newHash);
        return token;
    }

    public async Task<TokenModel> Login(Guid userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new ServerExceptions(StatusCode.UserNotFound, $"User not found: {userId}");
        }

        var oldHash = user.RefreshTokenHash;
        if (!string.IsNullOrEmpty(oldHash))
        {
            _postCommit.Enqueue(async _ => await _redis.KeyDeleteAsync(TokenHashKey(oldHash)));
        }

        var token = _jwtAppService.CreateToken(user.Id);
        var newHash = _jwtAppService.HashToken(token.RefreshToken);

        user.RefreshTokenHash = newHash;
        user.RefreshTokenExpiredAt = DateTime.UtcNow.AddSeconds(token.RefreshTokenLifetimeSeconds);

        var expiry = TimeSpan.FromSeconds(token.RefreshTokenLifetimeSeconds);
        _postCommit.Enqueue(async _ => await _redis.StringSetAsync(TokenHashKey(newHash), user.Id.ToString(), expiry));

        _logger.LogInformation("[Login] userId={UserId} newHash={New}", user.Id, newHash);
        return token;
    }

    public async Task<TokenModel> RefreshTokenAsync(string refreshToken)
    {
        var now = DateTime.UtcNow;
        var oldHash = _jwtAppService.HashToken(refreshToken);
        var oldKey = TokenHashKey(oldHash);

        var redisUserId = await _redis.StringGetAsync(oldKey);
        UserModel? user;

        if (redisUserId.HasValue)
        {
            if (!Guid.TryParse(redisUserId!, out var userId))
            {
                await _redis.KeyDeleteAsync(oldKey);
                throw new ServerExceptions(StatusCode.Unauthenticated, "Unauthorized");
            }

            user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
            {
                await _redis.KeyDeleteAsync(oldKey);
                throw new ServerExceptions(StatusCode.Unauthenticated, "Unauthorized");
            }

            if (!string.Equals(user.RefreshTokenHash, oldHash, StringComparison.Ordinal))
            {
                await _redis.KeyDeleteAsync(oldKey);
                throw new ServerExceptions(StatusCode.Unauthenticated, "Unauthorized");
            }
        }
        else
        {
            user = await _db.Users.FirstOrDefaultAsync(x => x.RefreshTokenHash == oldHash);
            if (user is null)
            {
                throw new ServerExceptions(StatusCode.Unauthenticated, "Unauthorized");
            }
        }

        if (user.RefreshTokenExpiredAt <= now)
        {
            if (redisUserId.HasValue)
            {
                await _redis.KeyDeleteAsync(oldKey);
            }

            throw new ServerExceptions(StatusCode.Unauthenticated, "Unauthorized");
        }

        var token = _jwtAppService.CreateToken(user.Id);
        var newHash = _jwtAppService.HashToken(token.RefreshToken);

        user.RefreshTokenHash = newHash;
        user.RefreshTokenExpiredAt = now.AddSeconds(token.RefreshTokenLifetimeSeconds);

        var expiry = TimeSpan.FromSeconds(token.RefreshTokenLifetimeSeconds);
        _postCommit.Enqueue(async _ => await _redis.KeyDeleteAsync(oldKey));
        _postCommit.Enqueue(async _ => await _redis.StringSetAsync(TokenHashKey(newHash), user.Id.ToString(), expiry));
        
        _logger.LogInformation("[RefreshToken] userId={UserId} oldHash={Old} --> newHash={New}", user.Id, oldHash, newHash);
        return token;
    }
}