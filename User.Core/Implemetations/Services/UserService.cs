using System.Security.Cryptography;
using System.Text;
using Auth.Core.Abstractions.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoList.Contracts.Requests.User;
using ToDoList.Contracts.Responses;
using ToDoList.Contracts.Responses.User;
using User.Core.Abstractions.Repositories;
using User.Core.Abstractions.Services;
using User.Core.Exceptions;

namespace User.Core.Implemetations.Services;

/// <inheritdoc cref="IUserService"/>
public class UserService : IUserService
{
    private readonly IBaseRepository<ToDoList.Contracts.Entities.User> _userRepository;
    private readonly ITokenCacheService _tokenCache;
    private readonly IMapper _mapper;
    
    public UserService(IBaseRepository<ToDoList.Contracts.Entities.User> userRepository, ITokenCacheService tokenCache, IMapper mapper)
    {
        _userRepository = userRepository;
        _tokenCache = tokenCache;
        _mapper = mapper;
    }

    public async Task<UserDescriptionResponse> Get(string? email, Guid? id)
    {
        // Инициализация переменной для хранения найденного пользователя
        ToDoList.Contracts.Entities.User? user = null;
        
        if (!string.IsNullOrWhiteSpace(email))
        {
            // Поиск пользователя по Email
            user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);
        }
        else if (id.HasValue)
        {
            // Поиск пользователя по ID
            user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }
        
        // Если пользователь не найден, выбрасывает исключение
        if (user is null)
        {
            throw new UserException("Пользователь с таким Email или Id не найден. Повторите попытку или зарегистрируйтесь.");
        }
        
        // Возвращает пользователя в виде списка из одного элемента через маппинг
        return _mapper.Map<UserDescriptionResponse>(user);
    }

    public async Task<UserDescriptionResponse> Update(PatchUserRequest request)
    {
        // Хешированние пароля
        var passwordHash = GetHashedPassword(request.Password);
        request.Password = passwordHash;
        
        // Проверка существования пользователя по ID, если такого нет - выбрасывает исключение
        var result = await _userRepository.GetById(request.Id);

        if (result is null)
        {
            throw new UserException("Пользователь с таким ID не найдет. Повторите попытку.");
        }
        
        // Обновляет поля пользователя
        result.Email = request.Email;
        result.Password = request.Password;
        result.FirstName = request.FirstName;
        result.LastName = request.LastName;
        
        result = await _userRepository.Update(result);
        
        // Возвращает обновлённые данные через маппинг
        return _mapper.Map<UserDescriptionResponse>(result);
    }

    public async Task<UserResponse> Delete(Guid id)
    {
        // Проверка существования пользователя по ID, если такого нет - выбрасывает исключение
        var result = await _userRepository.GetById(id);

        if (result is null)
        {
            throw new UserException("Пользователь с таким ID не найдет. Повторите попытку.");
        }
        
        // Удаляет пользователя
        result = await _userRepository.Delete(result);
        
        // Возвращает информацию об удалённом пользователе через маппинг
        return _mapper.Map<UserResponse>(result);
    }
    
    public async Task Logout(Guid id)
    {
        await _tokenCache.RevokeRefreshTokenAsync(id); // Удаление Refresh Token из Redis, делая его невалидным
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