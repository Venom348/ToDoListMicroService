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
var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен в формате: Bearer {ваш токен}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ✅ Redis (сначала!)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return ConnectionMultiplexer.Connect(config.GetConnectionString("Redis"));
});

// ✅ TokenCacheService
builder.Services.AddSingleton<ITokenCacheService, TokenCacheService>();

// JWT настройки
builder.Services.Configure<AuthOption>(builder.Configuration.GetSection("JwtSettings"));

// Validators
builder.Services.AddValidatorsFromAssembly(typeof(PostRegisterRequestValidator).Assembly);

// ✅ AutoMapper (ДОБАВЬТЕ!)
builder.Services.AddAutoMapper(opt => opt.AddProfile<AuthProfile>());

// Authentication & Authorization
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var authOptions = serviceProvider.GetRequiredService<IOptions<AuthOption>>().Value;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = authOptions.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });

// Сервисы
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddDbContext<ApplicationContext>(opt => 
    opt.UseNpgsql(configuration.GetConnectionString("Psql")));
builder.Services.AddTransient<IBaseRepository<User>, AuthRepository>();

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// ✅ ВАЖНО: Добавьте эти middleware В ПРАВИЛЬНОМ ПОРЯДКЕ
app.UseAuthentication(); // Сначала аутентификация
app.UseAuthorization();  // Потом авторизация

app.Run();