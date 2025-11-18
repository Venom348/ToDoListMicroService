using ToDoList.Contracts.Entities.Enums;

namespace ToDoList.Contracts.Requests.Task;

/// <summary>
///     Модель отправки задачи на сервер
/// </summary>
public class PostTaskRequest
{
    /// <summary>
    ///     Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    ///     Название задачи
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    ///     Описание задачи
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    ///     Статус задачи
    /// </summary>
    public Status Status { get; set; }
    
    /// <summary>
    ///     Приоритет задачи
    /// </summary>
    public Priority Priority { get; set; }
    
    /// <summary>
    ///     Дата и время изменения задачи
    /// </summary>
    public DateOnly ModifiedDate { get; set; }
}