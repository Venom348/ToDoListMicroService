using Microsoft.EntityFrameworkCore;
using User.Core.Abstractions.Repositories;

namespace User.Persistence.Repositories;

/// <inheritdoc cref="IBaseRepository{User}"/>
public class UserRepository : IBaseRepository<ToDoList.Contracts.Entities.User>
{
    private readonly ApplicationContext _dbContext;

    public UserRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    // Получение всех записей
    public IQueryable<ToDoList.Contracts.Entities.User> GetAll() => _dbContext.Users;
    
    // Получение по ID
    public async Task<ToDoList.Contracts.Entities.User> GetById(Guid id) => await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == id);
    
    // Изменение сущности
    public async Task<ToDoList.Contracts.Entities.User> Update(ToDoList.Contracts.Entities.User entity)
    {
        _dbContext.Users.Update(entity);
        await SaveChangesAsync();
        return entity;
    }
    
    // Удаление сущности
    public async Task<ToDoList.Contracts.Entities.User> Delete(ToDoList.Contracts.Entities.User entity)
    {
        _dbContext.Users.Remove(entity);
        await SaveChangesAsync();
        return entity;
    }
    
    // Метод сохранения
    public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}