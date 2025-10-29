using Microsoft.EntityFrameworkCore;
using Task.Core.Abstractions.Repositories;

namespace Task.Persistence.Repositories;

/// <inheritdoc cref="IBaseRepository{Task}"/>
public class TaskRepository : IBaseRepository<ToDoList.Contracts.Entities.Task>
{
    private readonly ApplicationContext _dbContext;

    public TaskRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    // Получение всех записей
    public IQueryable<ToDoList.Contracts.Entities.Task> GetAll() => _dbContext.Tasks;

    // Получение по ID
    public async Task<ToDoList.Contracts.Entities.Task> GetById(Guid id) => await _dbContext.Tasks.FirstOrDefaultAsync(s => s.Id == id);

    // Создание сущности
    public async Task<ToDoList.Contracts.Entities.Task> Create(ToDoList.Contracts.Entities.Task entity)
    {
        _dbContext.Tasks.Add(entity);
        await SaveChangesAsync();
        return entity;
    }
    
    // Изменение сущности
    public async Task<ToDoList.Contracts.Entities.Task> Update(ToDoList.Contracts.Entities.Task entity)
    {
        _dbContext.Tasks.Update(entity);
        await SaveChangesAsync();
        return entity;
    }
    
    // Удаление сущности
    public async Task<ToDoList.Contracts.Entities.Task> Delete(ToDoList.Contracts.Entities.Task entity)
    {
        _dbContext.Tasks.Remove(entity);
        await SaveChangesAsync();
        return entity;
    }
    
    // Метод сохранения
    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}