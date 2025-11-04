namespace ToDoList.Contracts.Requests.Auth;

/// <summary>
///     Запрос на обновление Access Token с использованием Refresh Token
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    ///     Refresh Token, полученный при авторизации или предыдущем обновлении токена
    /// </summary>
    public string RefreshToken { get; set; }
}