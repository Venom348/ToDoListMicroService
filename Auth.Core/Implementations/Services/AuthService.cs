using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Core.Abstractions.Repositories;
using Auth.Core.Abstractions.Services;
using Auth.Core.Exceptions;
using Auth.Core.Options;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Contracts.Entities;
using ToDoList.Contracts.Requests.Auth;
using ToDoList.Contracts.Responses.Auth;
using ToDoList.Contracts.Responses.User;
using Task = System.Threading.Tasks.Task;

namespace Auth.Core.Implementations.Services;

/// <inheritdoc cref="IAuthService"/>
public class AuthService : IAuthService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IOptions<AuthOption> _authOptions;
    private readonly ITokenCacheService _tokenCache;
    private readonly IMapper _mapper;

    public AuthService(IBaseRepository<User> userRepository, IOptions<AuthOption> authOptions, ITokenCacheService tokenCache, IMapper mapper)
    {
        _userRepository = userRepository;
        _authOptions = authOptions;
        _tokenCache = tokenCache;
        _mapper = mapper;
    }

    public async Task Register(PostRegisterRequest request)
    {
        // Проверка существования пользователя с таким же Email в базе данных
        var existingUser = await _userRepository.GetAll()
            .FirstOrDefaultAsync(u => u.Email == request.Email);
        
        // Если пользователь с таким Email уже существует, выбрасываем исключение
        if (existingUser != null)
        {
            throw new AuthException("Пользователь с таким Email уже существует.");
        }

        // Создание объекта нового пользователя на основе данных из запроса
        var result = new User
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            RegistrationDate = request.RegistrationDate,
        };
        
        // Хешированние пароля
        var passwordHash = GetHashedPassword(request.Password);
        result.Password = passwordHash;
        
        // Создание пользователя
        await _userRepository.Create(result);
    }

    public async Task<PostLoginResponse> Login(PostLoginRequest request)
    {
        // Хешированние пароля
        var passwordHash = GetHashedPassword(request.Password);
        request.Password = passwordHash;
        
        // Поиск пользователя по Email и хешу пароля
        var result = await _userRepository.GetAll()
            .FirstOrDefaultAsync(s => s.Email == request.Email && s.Password == passwordHash);
        
        // Проверка существования аккаунта, если такого нет - выбрасывает исключение
        if (result is null)
        {
            throw new AuthException("Аккаунт не найден. Повторите попытку или зарегистрируйтесь.");
        }

        var accessToken = GenerateAccessToken(result); // Генерация Access Token (короткоживущий токен для доступа к API)
        var refreshToken = GenerateRefreshToken(); // Генерация Refresh Token (долгоживущий токен для обновления Access Token)
        
        // Сохранение Refresh Token в Redis с временем жизни 7 дней
        await _tokenCache.StoreRefreshTokenAsync
        (
            result.Id,
            refreshToken,
            TimeSpan.FromDays(7)
        );
        
        // Возврат токенов и информации о пользователе
        return new PostLoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _mapper.Map<UserDescriptionResponse>(result), 
        };
    }

    public async Task<PostLoginResponse> RefreshToken(RefreshTokenRequest request, Guid id)
    {
        // Проверка валидности Refresh Token - сравнение с сохраненным в Redis
        var isValid = await _tokenCache.ValidateRefreshTokenAsync(id, request.RefreshToken);
        
        // Если токен невалиден или истек, выбрасываем исключение
        if (!isValid)
        {
            throw new AuthException("Недействительный Refresh Token");
        }
        
        // Получение данных пользователя из базы данных по идентификатору
        var result = await _userRepository.GetAll()
            .FirstOrDefaultAsync(s => s.Id == id);
        
        // Если пользователь не найден (удален из системы), выбрасываем исключение
        if (result is null)
        {
            throw new AuthException("Пользователь не найден");
        }
        
        // Генерация нового Access Token (старый истек)
        var newAccessToken = GenerateAccessToken(result);
        
        // Генерация нового Refresh Token (ротация токенов для безопасности)
        var newRefreshToken = GenerateRefreshToken();
        
        // Сохранение нового Refresh Token в Redis, заменяя старый
        await _tokenCache.StoreRefreshTokenAsync
        (
            result.Id,
            newRefreshToken,
            TimeSpan.FromDays(7)
        );
        
        // Возврат новой пары токенов и актуальной информации о пользователе
        return new PostLoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = _mapper.Map<UserDescriptionResponse>(result), 
        };
    }

    public async Task Logout(Guid id)
    {
        await _tokenCache.RevokeRefreshTokenAsync(id); // Удаление Refresh Token из Redis, делая его невалидным
    }

    // Генерация Access Token (JWT)
    private string GenerateAccessToken(User user)
    {
        // Формирование набора claims (утверждений) - информации о пользователе внутри токена
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email), // Email пользователя
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Уникальный идентификатор пользователя
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}") // Полное имя пользователя
        };
        
        // Создание JWT токена с указанными параметрами
        var jwt = new JwtSecurityToken(
            issuer: _authOptions.Value.Issuer, // Издатель токена (название приложения/сервиса)
            audience: _authOptions.Value.Audience, // Аудитория токена (для кого предназначен)
            claims: claims, // Встраиваемая информация о пользователе
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)), // Срок действия: 15 минут (короткоживущий для безопасности)
            signingCredentials: new SigningCredentials( // Параметры подписи токена
                _authOptions.Value.GetSymmetricSecurityKey(), // Секретный ключ для подписи
                SecurityAlgorithms.HmacSha256)); // Алгоритм подписи HMAC-SHA256
        
        // Преобразование JWT объекта в строку (сериализация)
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    // Генерация Refresh Token (случайная строка)
    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64]; // Массив для хранения случайных байтов (64 байта = 512 бит)
        using var rng = RandomNumberGenerator.Create(); // Использование криптографически стойкого генератора случайных чисел
        rng.GetBytes(randomBytes); // Заполнение массива случайными байтами
        return Convert.ToBase64String(randomBytes); // Преобразование байтов в Base64 строку для удобной передачи
    }
    
    /// <summary>
    ///     Метод для хешированния пароля
    /// </summary>
    /// <param name="password">Пароль пользователя</param>
    /// <returns></returns>
    private string GetHashedPassword(string password)
    {
        using var sha = SHA256.Create();
        var data = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(data);
    }
}