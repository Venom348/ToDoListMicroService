using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Core.Abstractions.Repositories;
using Auth.Core.Abstractions.Services;
using Auth.Core.Exceptions;
using Auth.Core.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Contracts.Entities;
using ToDoList.Contracts.Requests.Auth;
using Task = System.Threading.Tasks.Task;

namespace Auth.Core.Implementations.Services;

/// <inheritdoc cref="IAuthService"/>
public class AuthService : IAuthService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IOptions<AuthOption> _authOptions;

    public AuthService(IBaseRepository<User> userRepository, IOptions<AuthOption> authOptions)
    {
        _userRepository = userRepository;
        _authOptions = authOptions;
    }

    public async Task Register(PostRegisterRequest request)
    {
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

    public async Task<string> Login(PostLoginRequest request)
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
        
        var claims = new List<Claim> { new Claim(ClaimTypes.Email, request.Email) };
        
        // Создание JWT-токена
        var jwt = new JwtSecurityToken(
            issuer: _authOptions.Value.Issuer,
            audience: _authOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
            signingCredentials: new SigningCredentials(_authOptions.Value.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        // Возвращает JWT-токен
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    /// <summary>
    ///     Метод для хешированния пароля
    /// </summary>
    /// <param name="password">Пароль пользователя</param>
    /// <returns></returns>
    private string GetHashedPassword(string password)
    {
        using var sha = SHA256.Create();
        var data = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
        return Encoding.ASCII.GetString(data);
    }
}