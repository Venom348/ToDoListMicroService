using Auth.Core.Abstractions.Services;
using StackExchange.Redis;

namespace Auth.Core.Implementations.Services;

/// <inheritdoc cref="ITokenCacheService"/>
public class TokenCacheService : ITokenCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public TokenCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    // Сохранение refresh токен в Redis с автоматическим истечением
    public async Task StoreRefreshTokenAsync(Guid id, string refreshToken, TimeSpan expiration)
    {
        var key = GetRefreshTokenKey(id); // Формирование уникального ключа для токена пользователя
        await _db.StringSetAsync(key, refreshToken, expiration); // Сохранение токена в Redis с TTL (время автоматического удаления)
    }
    
    // Извлечение refresh токен из Redis по идентификатору пользователя
    public async Task<string?> GetRefreshTokenAsync(Guid id)
    {
        var key = GetRefreshTokenKey(id); // Формирование ключа для поиска токена
        var token = await _db.StringGetAsync(key); // Получение значения из Redis
        return token.HasValue ? token.ToString() : null; // Возврат токена, если он существует, иначе null
    }
    
    // Удаление refresh токен из Redis (инвалидация токена)
    public async Task RevokeRefreshTokenAsync(Guid id)
    {
        var key = GetRefreshTokenKey(id); // Формирование ключа токена для удаления
        await _db.KeyDeleteAsync(key); // Удаление ключа из Redis
    }
    
    // Проверка совпадения предоставленного токена с сохраненным в Redis
    public async Task<bool> ValidateRefreshTokenAsync(Guid id, string refreshToken)
    {
        var storedToken = await GetRefreshTokenAsync(id); // Получение сохраненного токена из кеша
        return storedToken == refreshToken; // Сравнение токенов: валиден, если совпадают
    }
    
    /// <summary>
    ///     Генерирует стандартизированный ключ Redis для refresh токена пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns></returns>
    private static string GetRefreshTokenKey(Guid id) => $"refresh_token:{id}";
}