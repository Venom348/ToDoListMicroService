namespace ToDoList.Contracts.Requests.User;

/// <summary>
///     Модель отправки пользователя на сервер
/// </summary>
public class PostUserRequest
{
    /// <summary>
    ///     Email пользователя
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    ///     Пароль пользователя
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    ///     Имя пользователя
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    ///     Фамилия пользователя
    /// </summary>
    public string LastName { get; set; }
}