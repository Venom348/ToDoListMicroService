using Microsoft.EntityFrameworkCore;
using ToDoList.Contracts.Entities;

namespace Task.Persistence;

/// <summary>
///     Модель для подключения к БД (Task)
/// </summary>
public class ApplicationContext : DbContext
{
    // Определение сущности
    public DbSet<ToDoList.Contracts.Entities.Task> Tasks => Set<ToDoList.Contracts.Entities.Task>();
    public DbSet<UserTask> UserTasks => Set<UserTask>();

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
    }

    private void Init()
    {
        SaveChanges();
    }
}