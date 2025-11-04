using ToDoList.Contracts.Requests.Auth;
using ToDoList.Contracts.Responses.Auth;

namespace Auth.Core.Abstractions.Services;

/// <summary>
///     Сервис для аутентификации пользователя
/// </summary>
public interface IAuthService
{
    /// <summary>
    ///     Регистрация пользователя
    /// </summary>
    /// <param name="request">Данные для регистрации пользователя</param>
    /// <returns></returns>
    Task Register(PostRegisterRequest request);
    
    /// <summary>
    ///     Авторизация пользователя
    /// </summary>
    /// <param name="request">Данные для авторизации пользователя</param>
    /// <returns></returns>
    Task<PostLoginResponse> Login(PostLoginRequest request);
    
    /// <summary>
    ///     Обновление токена доступа с использованием refresh токена
    /// </summary>
    /// <param name="request">Запрос с refresh токеном для получения новой пары токенов</param>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns></returns>
    Task<PostLoginResponse> RefreshToken(RefreshTokenRequest request, Guid id);
    
    /// <summary>
    ///     Выход пользователя из системы (инвалидация токенов)
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns></returns>
    Task Logout(Guid id);
}