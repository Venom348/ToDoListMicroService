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
        var existingUser = await _userRepository.GetAll()
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            throw new AuthException("Пользователь с таким Email уже существует.");
        }

        // Переданные данные для регистрации пользователя
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
        
        // Данные для авторизации пользователя
        var result = await _userRepository.GetAll()
            .FirstOrDefaultAsync(s => s.Email == request.Email && s.Password == passwordHash);
        
        // Проверка существования аккаунта, если такого нет - выбрасывает исключение
        if (result is null)
        {
            throw new AuthException("Аккаунт не найден. Повторите попытку или зарегистрируйтесь.");
        }

        var accessToken = GenerateAccessToken(result);
        var refreshToken = GenerateRefreshToken();

        await _tokenCache.StoreRefreshTokenAsync
        (
            result.Id,
            refreshToken,
            TimeSpan.FromDays(7)
        );

        return new PostLoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _mapper.Map<UserDescriptionResponse>(result), 
        };
    }

    public async Task<PostLoginResponse> RefreshToken(RefreshTokenRequest request, Guid id)
    {
        var isValid = await _tokenCache.ValidateRefreshTokenAsync(id, request.RefreshToken);

        if (!isValid)
        {
            throw new AuthException("Недействительный Refresh Token");
        }
        
        var result = await _userRepository.GetAll()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (result is null)
        {
            throw new AuthException("Пользователь не найден");
        }
        
        // Генерация нового Access Token
        var newAccessToken = GenerateAccessToken(result);
        
        // Генерация нового Refresh Token
        var newRefreshToken = GenerateRefreshToken();

        await _tokenCache.StoreRefreshTokenAsync
        (
            result.Id,
            newRefreshToken,
            TimeSpan.FromDays(7)
        );
        
        return new PostLoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = _mapper.Map<UserDescriptionResponse>(result), 
        };
    }

    public async Task Logout(Guid id)
    {
        await _tokenCache.RevokeRefreshTokenAsync(id);
    }

    // Генерация Access Token (JWT)
    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };
        
        var jwt = new JwtSecurityToken(
            issuer: _authOptions.Value.Issuer,
            audience: _authOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)), // 15 минут для Access Token
            signingCredentials: new SigningCredentials(
                _authOptions.Value.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    // Генерация Refresh Token (случайная строка)
    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
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