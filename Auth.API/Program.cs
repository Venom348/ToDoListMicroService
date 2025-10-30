using Auth.Core.Abstractions.Repositories;
using Auth.Core.Abstractions.Services;
using Auth.Core.Implementations.Services;
using Auth.Core.Options;
using Auth.Core.Validations;
using Auth.Persistence;
using Auth.Persistence.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Contracts.Entities;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Получение доступа к конфигурации

builder.Services.AddControllers(); // Регистрирует сервисы для работы с контроллерами
builder.Services.AddSwaggerGen(); // Генерация документации Swagger

// Подключение JWT-токена через конфигурацию
builder.Services.Configure<AuthOption>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddValidatorsFromAssembly(typeof(PostRegisterRequestValidator).Assembly); // Регистрируем Validator

// Добавление сервиса авторизации в контейнер зависимостей
builder.Services.AddAuthorization();
// Добавление сервиса аутентификации с использованием JWT Bearer токена
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Создание временного поставщика сервисов для получения зарегистрированных служб
        var serviceProvider = builder.Services.BuildServiceProvider();
        // Получение настроек класса AuthOptions из контейнера зависимостей
        var authOptions = serviceProvider.GetRequiredService<IOptions<AuthOption>>().Value;
        
        // Настройка параметров валидации JWT-токена
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = authOptions.Issuer,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = authOptions.Audience,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Psql")));
builder.Services.AddTransient<IBaseRepository<User>, AuthRepository>();

var app = builder.Build();

app.MapControllers(); // Машрутизация контроллеров
app.UseSwagger(); // Включает middleware для генерации Swagger JSON
app.UseSwaggerUI(); // Включает веб-интерфейс для документации API
app.UseHttpsRedirection(); // Перенаправляет HTTP запросы на HTTPS

app.Run();