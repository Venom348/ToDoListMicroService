using ToDoList.Contracts.Common;

namespace Task.Core.Abstractions.Repositories;

/// <summary>
///     Базовый интерфейс для взаимодействия с сущностями в БД
/// </summary>
public interface IBaseRepository<TEntity> where TEntity : Entity
{
    /// <summary>
    ///     Получаем запрос для entity
    /// </summary>
    IQueryable<TEntity> GetAll();
    
    /// <summary>
    ///     Получаем сущность по ID
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    Task<TEntity> GetById(Guid id);
    
    /// <summary>
    ///     Создаём сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    Task<TEntity> Create(TEntity entity);
    
    /// <summary>
    ///     Обновляем сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task<TEntity> Update(TEntity entity);
    
    /// <summary>
    ///     Удаляем сущность
    /// </summary>
    /// <param name="entity">Сущность</param>
    /// <returns></returns>
    Task<TEntity> Delete(TEntity entity);
    
    /// <summary>
    ///     Асинхронный вспомогательный метод сохранения
    /// </summary>
    Task<int> SaveChangesAsync();
}