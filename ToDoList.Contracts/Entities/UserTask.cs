using System.ComponentModel.DataAnnotations;
using ToDoList.Contracts.Common;

namespace ToDoList.Contracts.Entities;

/// <summary>
///     Сущность для связи пользователя и задачи (промежуточная таблица many-to-many)
/// </summary>
public class UserTask : Entity
{
    /// <summary>
    ///     Идентификатор пользователя, связанного с задачей
    /// </summary>
    public Guid? UserId { get; set; }
    
    /// <summary>
    ///     Навигационное свойство для связанной задачи
    /// </summary>
    public Task Task { get; set; }
}