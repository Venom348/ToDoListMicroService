using ToDoList.Contracts.Common;
using ToDoList.Contracts.Entities.Enums;

namespace ToDoList.Contracts.Entities;

/// <summary>
///     Модель задачи
/// </summary>
public class Task : Entity
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
    ///     Дата создания задачи
    /// </summary>
    public DateOnly CreatedDate { get; set; }
    
    /// <summary>
    ///     Дата и время изменения задачи
    /// </summary>
    public DateOnly ModifiedDate { get; set; }
}