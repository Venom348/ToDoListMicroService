namespace ToDoList.Contracts.Requests.Auth;

/// <summary>
///     Модель отправки авторизации на сервер
/// </summary>
public class PostLoginRequest
{
    /// <summary>
    ///     Email пользователя
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    ///     Пароль пользователя
    /// </summary>
    public string Password { get; set; }
}