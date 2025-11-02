using Auth.Core.Abstractions.Services;
using StackExchange.Redis;

namespace Auth.Core.Implementations.Services;

public class TokenCacheService : ITokenCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public TokenCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task StoreRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan expiration)
    {
        var key = GetRefreshTokenKey(userId);
        await _db.StringSetAsync(key, refreshToken, expiration);
    }

    public async Task<string?> GetRefreshTokenAsync(Guid userId)
    {
        var key = GetRefreshTokenKey(userId);
        var token = await _db.StringGetAsync(key);
        return token.HasValue ? token.ToString() : null;
    }

    public async Task RevokeRefreshTokenAsync(Guid userId)
    {
        var key = GetRefreshTokenKey(userId);
        await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var storedToken = await GetRefreshTokenAsync(userId);
        return storedToken == refreshToken;
    }
    
    private static string GetRefreshTokenKey(Guid userId) => $"refresh_token:{userId}";
}