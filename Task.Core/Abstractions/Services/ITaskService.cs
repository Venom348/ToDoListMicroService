using ToDoList.Contracts.Requests.Task;
using ToDoList.Contracts.Responses;
using ToDoList.Contracts.Responses.Task;

namespace Task.Core.Abstractions.Services;

/// <summary>
///     Сервис для работы с задачами
/// </summary>
public interface ITaskService
{
    /// <summary>
    ///     Получение задачи
    /// </summary>
    /// <param name="title">Название задачи</param>
    /// <param name="page">Страница для пагинации</param>
    /// <param name="limit">Лимит пагинации</param>
    /// <returns></returns>
    Task<List<TaskDescriptionResponse>> Get(string title, int page = 0, int limit = 20);
    
    /// <summary>
    ///     Создание задачи
    /// </summary>
    /// <param name="request">Данные для создания задачи</param>
    /// <returns></returns>
    Task<TaskDescriptionResponse> Create(PostTaskRequest request);
    
    /// <summary>
    ///     Обновление задачи
    /// </summary>
    /// <param name="request">Данные для обновления задачи</param>
    /// <returns></returns>
    Task<TaskDescriptionResponse> Update(PatchTaskRequest request);
    
    /// <summary>
    ///     Удаление задачи
    /// </summary>
    /// <param name="id">Идентификатор задачи</param>
    /// <returns></returns>
    Task<TaskResponse> Delete(Guid id);
}