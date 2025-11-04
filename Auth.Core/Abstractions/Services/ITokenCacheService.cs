namespace Auth.Core.Abstractions.Services;

/// <summary>
///     Сервис для управления кешированием refresh токенов в Redis
/// </summary>
public interface ITokenCacheService
{
    /// <summary>
    ///     Сохранение refresh токена в кеш с указанным временем жизни
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="refreshToken">Refresh токен для сохранения</param>
    /// <param name="expiration">Время жизни токена в кеше</param>
    /// <returns></returns>
    Task StoreRefreshTokenAsync(Guid id, string refreshToken, TimeSpan expiration);
    
    /// <summary>
    ///     Получение refresh токена из кеша по идентификатору пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns></returns>
    Task<string?>  GetRefreshTokenAsync(Guid id);
    
    /// <summary>
    ///     Удаление refresh токена из кеша (используется при logout)
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns></returns>
    Task RevokeRefreshTokenAsync(Guid id);
    
    /// <summary>
    ///     Проверка валидности refresh токена - сравнение токена из запроса с сохраненным в кеше
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="refreshToken">Refresh токен для проверки</param>
    /// <returns></returns>
    Task<bool> ValidateRefreshTokenAsync(Guid id, string refreshToken);
}