using Auth.Core.Abstractions.Repositories;
using Auth.Core.Abstractions.Services;
using Auth.Core.Implementations.Services;
using Auth.Core.Mapping;
using Auth.Core.Options;
using Auth.Core.Validations;
using Auth.Persistence;
using Auth.Persistence.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using ToDoList.Contracts.Entities;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Получение доступа к конфигурации

builder.Services.AddControllers(); // Регистрирует сервисы для работы с контроллерами

// Генерация документации Swagger
builder.Services.AddSwaggerGen(options =>
{
    // Определение схемы безопасности Bearer (JWT) для Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // Имя заголовка для токена
        Type = SecuritySchemeType.Http, // Тип схемы безопасности
        Scheme = "Bearer", // Схема аутентификации
        BearerFormat = "JWT", // Формат токена
        In = ParameterLocation.Header, // Токен передается в заголовке
        Description = "Введите JWT токен в формате: {ваш токен}"
    });
    
    // Добавление требования безопасности для всех эндпоинтов в Swagger
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Ссылка на схему безопасности
                }
            },
            Array.Empty<string>() // Пустой массив scopes (для JWT не требуются конкретные scope)
        }
    });
});

// Регистрация IConnectionMultiplexer как Singleton для повторного использования соединения
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    // Получение строки подключения из конфигурации и создание соединения с Redis
    return ConnectionMultiplexer.Connect(config.GetConnectionString("Redis"));
});

builder.Services.AddSingleton<ITokenCacheService, TokenCacheService>(); // Регистрация сервиса кеширования токенов

builder.Services.Configure<AuthOption>(builder.Configuration.GetSection("JwtSettings")); // Подключение JWT настроек из секции конфигурации "JwtSettings"
builder.Services.AddValidatorsFromAssembly(typeof(PostRegisterRequestValidator).Assembly); // Регистрируем Validator для регистрации
builder.Services.AddValidatorsFromAssembly(typeof(PostLoginRequestValidator).Assembly); // Регистрируем Validator для авторизации
builder.Services.AddAutoMapper(opt => opt.AddProfile<AuthProfile>()); // Добавление профиля для автомаппера

// Добавление поддержки авторизации
builder.Services.AddAuthorization();
// Настройка аутентификации с использованием JWT Bearer токенов
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Получение настроек JWT из конфигурации
        var serviceProvider = builder.Services.BuildServiceProvider();
        var authOptions = serviceProvider.GetRequiredService<IOptions<AuthOption>>().Value;
        
        // Параметры валидации JWT токенов
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Проверять издателя токена
            ValidIssuer = authOptions.Issuer, // Ожидаемый издатель
            ValidateAudience = true, // Проверять аудиторию токена
            ValidAudience = authOptions.Audience, // Ожидаемая аудитория
            ValidateLifetime = true, // Проверять срок действия токена
            IssuerSigningKey = authOptions.GetSymmetricSecurityKey(), // Ключ для проверки подписи
            ValidateIssuerSigningKey = true, // Проверять подпись токена
        };
    });

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Psql")));
builder.Services.AddTransient<IBaseRepository<User>, AuthRepository>();

var app = builder.Build();

app.MapControllers(); // Маршрутизация контроллеров
app.UseSwagger(); // Включает middleware для генерации Swagger JSON
app.UseSwaggerUI(); // Включает веб-интерфейс для документации API
app.UseHttpsRedirection(); // Перенаправляет HTTP запросы на HTTPS
app.UseAuthentication(); // Сначала аутентификация
app.UseAuthorization();  // Потом авторизация

app.Run();