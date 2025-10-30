using Microsoft.EntityFrameworkCore;
using ToDoList.Contracts.Entities;

namespace Auth.Persistence;

/// <summary>
///     Модель для подключения к БД (Auth)
/// </summary>
public class ApplicationContext : DbContext
{
    // Определение сущности
    public DbSet<User> Users => Set<User>();

    public ApplicationContext(DbContextOptions options) : base(options)
    {
        // Проверяет, существует ли БД
        // Если нет, то создаёт её
        // Если существует - возвращает false
        if (Database.EnsureCreated())
        {
            Init();
        }
    }
    
    private void Init()
    {
        SaveChanges();
    }
}