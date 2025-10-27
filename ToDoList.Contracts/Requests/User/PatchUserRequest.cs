namespace ToDoList.Contracts.Requests.User;

/// <summary>
///     Модель изменения пользователя на сервере
/// </summary>
public class PatchUserRequest : PostUserRequest
{
    /// <summary>
    ///     Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }
}