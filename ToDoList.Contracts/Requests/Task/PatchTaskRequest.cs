namespace ToDoList.Contracts.Requests.Task;

/// <summary>
///     Модель изменения задачи на сервере
/// </summary>
public class PatchTaskRequest : PostTaskRequest
{
    /// <summary>
    ///     Идентификатор задачи
    /// </summary>
    public Guid Id { get; set; }
}