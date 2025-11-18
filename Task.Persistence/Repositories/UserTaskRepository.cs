using Microsoft.EntityFrameworkCore;
using Task.Core.Abstractions.Repositories;
using ToDoList.Contracts.Entities;

namespace Task.Persistence.Repositories;

/// <summary>
///     Репозиторий для работы с сущностью UserTask (связь пользователей и задач)
/// </summary>
public class UserTaskRepository : IBaseRepository<UserTask>
{
    private readonly ApplicationContext _dbContext;

    public UserTaskRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    // Получение всех записей
    public IQueryable<UserTask> GetAll() => _dbContext.UserTasks;

    // Получение записи UserTask по идентификатору
    public async Task<UserTask> GetById(Guid id) => await _dbContext.UserTasks.FirstOrDefaultAsync(x => x.Id == id);
    
    // Создание новой записи UserTask
    public async Task<UserTask> Create(UserTask entity)
    {
        _dbContext.UserTasks.Add(entity);
        await  SaveChangesAsync();
        return entity;
    }
    
    // Обновление существующей записи UserTask
    public async Task<UserTask> Update(UserTask entity)
    {
        _dbContext.UserTasks.Update(entity);
        await  SaveChangesAsync();
        return entity;
    }
    
    // Удаление записи UserTask
    public async Task<UserTask> Delete(UserTask entity)
    {
        _dbContext.UserTasks.Remove(entity);
        await  SaveChangesAsync();
        return entity;
    }

    // Метод сохранения
    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}