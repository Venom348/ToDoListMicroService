using Microsoft.EntityFrameworkCore;

namespace Task.Persistence;

/// <summary>
///     Модель для подключения к БД (Task)
/// </summary>
public class ApplicationContext : DbContext
{
    // Определение сущности
    public DbSet<ToDoList.Contracts.Entities.Task> Tasks => Set<ToDoList.Contracts.Entities.Task>();

    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
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