using ToDoList.Contracts.Entities.Enums;

namespace ToDoList.Contracts.Responses.Task;

/// <summary>
///     Класс для представления информации о задачи
/// </summary>
public class TaskDescriptionResponse : TaskResponse
{
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