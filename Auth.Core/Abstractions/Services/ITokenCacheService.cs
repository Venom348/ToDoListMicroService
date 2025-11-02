namespace Auth.Core.Abstractions.Services;

public interface ITokenCacheService
{
    Task StoreRefreshTokenAsync(Guid userId, string refreshToken, TimeSpan expiration);
    Task<string?>  GetRefreshTokenAsync(Guid userId);
    Task RevokeRefreshTokenAsync(Guid userId);
    Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);
}