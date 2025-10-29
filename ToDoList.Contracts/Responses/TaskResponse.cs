namespace ToDoList.Contracts.Responses;

/// <summary>
///     Краткая информация о задачи
/// </summary>
public class TaskResponse
{
    /// <summary>
    ///     Идентификатор задачи
    /// </summary>
    public Guid Id { get; set; }
}