using Auth.Core.Abstractions.Repositories;
using ToDoList.Contracts.Entities;

namespace Auth.Persistence.Repositories;

public class AuthRepository : IBaseRepository<User>
{
    private readonly ApplicationContext _dbContext;
    
    public AuthRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Получение всех записей
    public IQueryable<User> GetAll() => _dbContext.Users;

    // Создание сущности
    public async Task<User> Create(User entity)
    {
        _dbContext.Users.Add(entity);
        await SaveChangesAsync();
        return entity;
    }
    
    // Метод сохранения
    public async Task<int> SaveChangesAsync() =>  await _dbContext.SaveChangesAsync();
}