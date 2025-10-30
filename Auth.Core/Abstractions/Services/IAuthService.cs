using ToDoList.Contracts.Requests.Auth;

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
    Task<string> Login(PostLoginRequest request);
}