using ToDoList.Contracts.Common;

namespace ToDoList.Contracts.Entities;

/// <summary>
///     Модель пользователя
/// </summary>
public class User : Entity
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
    
    /// <summary>
    ///     Дата регистрации
    /// </summary>
    public DateOnly RegistrationDate { get; set; }
}