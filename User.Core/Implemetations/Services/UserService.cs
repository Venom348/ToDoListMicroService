using System.Security.Cryptography;
using System.Text;
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
    private readonly IMapper  _mapper;
    
    public UserService(IBaseRepository<ToDoList.Contracts.Entities.User> userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<UserDescriptionResponse>> Get(string email)
    {
        // Проверка входных данных
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email не может быть пустым.", nameof(email));
        }
        
        // Поиск пользователя по Email
        var result = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);
        
        // Если Email не найден, выбрасывает исключение
        if (result is null)
        {
            throw new UserException("Пользователь с таким Email не найден. Повторите попытку или зарегистрируйтесь.");
        }
        
        // Возвращает пользователя в виде списка из одного элемента через маппинг
        return new List<UserDescriptionResponse>([_mapper.Map<UserDescriptionResponse>(result)]);
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