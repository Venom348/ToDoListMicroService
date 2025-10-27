using Microsoft.EntityFrameworkCore;
using User.Core.Abstractions.Repositories;
using User.Core.Abstractions.Services;
using User.Core.Implemetations.Services;
using User.Core.Mapping;
using User.Persistence;
using User.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Получение доступа к конфигурации

builder.Services.AddControllers(); // Регистрирует сервисы для работы с контроллерами
builder.Services.AddSwaggerGen(); // Генерация документации Swagger

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Psql")));
builder.Services.AddTransient<IBaseRepository<ToDoList.Contracts.Entities.User>, UserRepository>();

// Добавление профиля для автомаппера
builder.Services.AddAutoMapper(opt => opt.AddProfile<UserProfile>());

var app = builder.Build();

app.MapControllers(); // Машрутизация контроллеров
app.UseSwagger(); // Включает middleware для генерации Swagger JSON
app.UseSwaggerUI(); // Включает веб-интерфейс для документации API
app.UseHttpsRedirection(); // Перенаправляет HTTP запросы на HTTPS

app.Run();