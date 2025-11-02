using Microsoft.EntityFrameworkCore;
using User.Core.Abstractions.Repositories;
using User.Core.Abstractions.Services;
using User.Core.Implemetations.Services;
using User.Core.Mapping;
using User.Persistence;
using User.Persistence.Repositories;
using FluentValidation;
using ToDoList.Contracts.Requests.User;
using User.Core.Validations;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Получение доступа к конфигурации

builder.Services.AddControllers(); // Регистрирует сервисы для работы с контроллерами
builder.Services.AddSwaggerGen(); // Генерация документации Swagger

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Psql")));
builder.Services.AddTransient<IBaseRepository<ToDoList.Contracts.Entities.User>, UserRepository>();
builder.Services.AddScoped<IValidator<PostUserRequest>, PostUserRequestValidator>(); 

// Добавление профиля для автомаппера
builder.Services.AddAutoMapper(opt => opt.AddProfile<UserProfile>());

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return ConnectionMultiplexer.Connect(config.GetConnectionString("Redis"));
});

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    var redis = app.Services.GetRequiredService<IConnectionMultiplexer>();
    var db = redis.GetDatabase();

    await db.StringSetAsync("test", "Hello from Redis via Rider");
    var value = await db.StringGetAsync("test");

    Console.WriteLine($"Redis ответил: {value}");
});

app.MapControllers(); // Машрутизация контроллеров
app.UseSwagger(); // Включает middleware для генерации Swagger JSON
app.UseSwaggerUI(); // Включает веб-интерфейс для документации API
app.UseHttpsRedirection(); // Перенаправляет HTTP запросы на HTTPS

app.Run();