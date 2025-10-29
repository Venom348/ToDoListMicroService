using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Task.Core.Abstractions.Repositories;
using Task.Core.Abstractions.Services;
using Task.Core.Implementations.Services;
using Task.Core.Mapping;
using Task.Core.Validations;
using Task.Persistence;
using Task.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Получение доступа к конфигурации

builder.Services.AddControllers(); // Регистрирует сервисы для работы с контроллерами
builder.Services.AddSwaggerGen(); // Генерация документации Swagger

builder.Services.AddValidatorsFromAssembly(typeof(PostTaskRequestValidator).Assembly); // Регистрируем Validator

builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Psql")));
builder.Services.AddTransient<IBaseRepository<ToDoList.Contracts.Entities.Task>, TaskRepository>();

// Добавление профиля для автомаппера
builder.Services.AddAutoMapper(opt => opt.AddProfile<TaskProfile>());

var app = builder.Build();

app.MapControllers(); // Машрутизация контроллеров
app.UseSwagger(); // Включает middleware для генерации Swagger JSON
app.UseSwaggerUI(); // Включает веб-интерфейс для документации API
app.UseHttpsRedirection(); // Перенаправляет HTTP запросы на HTTPS

app.Run();