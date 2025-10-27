using ToDoList.Contracts.Requests.User;
using ToDoList.Contracts.Responses;
using ToDoList.Contracts.Responses.User;

namespace User.Core.Abstractions.Services;

/// <summary>
///     Сервис для работы с пользователями
/// </summary>
public interface IUserService
{
    /// <summary>
    ///     Получение пользователя из БД
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <returns></returns>
    Task<List<UserDescriptionResponse>> Get(string email);
    
    /// <summary>
    ///     Обновление данных пользователя
    /// </summary>
    /// <param name="request">Данные об обновлении пользователя</param>
    /// <returns></returns>
    Task<UserDescriptionResponse> Update(PatchUserRequest  request);
    
    /// <summary>
    ///     Удаление пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns></returns>
    Task<UserResponse> Delete(Guid id);
}