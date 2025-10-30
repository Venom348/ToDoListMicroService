using ToDoList.Contracts.Common;

namespace Auth.Core.Abstractions.Repositories;

/// <summary>
///     Базовый интерфейс для взаимодействия с сущностями в БД
/// </summary>
public interface IBaseRepository<TEntity> where TEntity : Entity
{
    /// <summary>
    ///     Получаем запрос для entity
    /// </summary>
    /// <returns></returns>
    IQueryable<TEntity> GetAll();
    
    /// <summary>
    ///     Добавляем новую сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task<TEntity> Create(TEntity entity);
    
    /// <summary>
    /// Асинхронный вспомогательный метод сохранения
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChangesAsync();
}