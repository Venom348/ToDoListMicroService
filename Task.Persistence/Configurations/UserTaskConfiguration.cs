using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Contracts.Entities;

namespace Task.Persistence.Configurations;

/// <summary>
///     Конфигурация Entity Framework для сущности UserTask
/// </summary>
public class UserTaskConfiguration : IEntityTypeConfiguration<UserTask>
{
    /// <summary>
    ///     Настройка маппинга сущности UserTask для Entity Framework Core
    /// </summary>
    /// <param name="builder">Строитель конфигурации для определения схемы таблицы и отношений</param>
    public void Configure(EntityTypeBuilder<UserTask> builder)
    {
        // Устанавливает первичный ключ таблицы на поле Id
        builder.HasKey(x => x.Id);
        
        // Создаёт уникальный индекс на поле UserId для обеспечения уникальности пользователя
        // Гарантирует, что один пользователь может иметь только одну связь UserTask
        builder.HasIndex(x => x.UserId).IsUnique();
    }
}