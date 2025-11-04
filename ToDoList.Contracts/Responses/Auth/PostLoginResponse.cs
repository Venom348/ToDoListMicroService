using ToDoList.Contracts.Responses.User;

namespace ToDoList.Contracts.Responses.Auth;

/// <summary>
///     Ответ на запрос авторизации пользователя
/// </summary>
public class PostLoginResponse
{
    /// <summary>
    ///     Access Token (JWT) - короткоживущий токен для доступа к защищенным эндпоинтам API
    /// </summary>
    public string AccessToken { get; set; }
    
    /// <summary>
    ///     Refresh Token - долгоживущий токен для обновления Access Token
    /// </summary>
    public string RefreshToken { get; set; }
    
    /// <summary>
    ///     Информация об авторизованном пользователе
    /// </summary>
    public UserDescriptionResponse User { get; set; }
}