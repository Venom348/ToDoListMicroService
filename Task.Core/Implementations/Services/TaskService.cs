using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Task.Core.Abstractions.Repositories;
using Task.Core.Abstractions.Services;
using Task.Core.Exceptions;
using ToDoList.Contracts.Requests.Task;
using ToDoList.Contracts.Responses;
using ToDoList.Contracts.Responses.Task;

namespace Task.Core.Implementations.Services;

/// <inheritdoc cref="ITaskService"/>
public class TaskService : ITaskService
{
    private readonly IBaseRepository<ToDoList.Contracts.Entities.Task>  _taskRepository;
    private readonly IMapper _mapper;

    public TaskService(IBaseRepository<ToDoList.Contracts.Entities.Task> taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<List<TaskDescriptionResponse>> Get(string? title, int page = 0, int limit = 20)
    {
        // Валидация параметров пагинации
        if (page < 0)
        {
            throw new TaskException("Номер страницы не должен быть отрицательным");
        }

        if (limit <= 0 || limit > 100)
        {
            throw new TaskException("Лимит должен быть от 1 до 100.");
        }
    
        // Если передан title - фильтруем по нему
        if (!string.IsNullOrWhiteSpace(title))
        {
            // Получение данных из БД
            var result = await _taskRepository.GetAll()
                .Where(x => x.Title.Contains(title))
                .Skip(page * limit)
                .Take(limit)
                .ToListAsync(); 
        
            // Если по названию задачи ничего не найдено - выбрасывает исключение
            if (result.Count == 0)
            {
                throw new TaskException("Задача с таким названием не найдена. Повторите попытку.");
            }
        
            // Возвращает список задач по полю title
            return _mapper.Map<List<TaskDescriptionResponse>>(result);
        }
    
        // Если название задачи не передано, то возвращает список всех задач
        var queryResult = await _taskRepository.GetAll()
            .Skip(page * limit)
            .Take(limit)
            .ToListAsync();
    
        // Если результат пустой - выбрасывает исключение
        if (queryResult.Count == 0)
        {
            throw new TaskException("Результат не найден. Повторите попытку");
        }
    
        // Возвращает список всех задач через маппинг
        return _mapper.Map<List<TaskDescriptionResponse>>(queryResult);
    }

    public async Task<TaskDescriptionResponse> Create(PostTaskRequest request)
    {
        // Создание задачи с переданными данными
        var result = await _taskRepository.Create(new ToDoList.Contracts.Entities.Task
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority
        });
        
        // Возвращает информацию о созданной задачи
        return _mapper.Map<TaskDescriptionResponse>(result);
    }

    public async Task<TaskDescriptionResponse> Update(PatchTaskRequest request)
    {
        // Проверка существования задачи по ID, если такой нет - выбрасывает исключение
        var result = await _taskRepository.GetById(request.Id);

        if (result is null)
        {
            throw new TaskException("Задача с таким ID не найдена. Повторите попытку");
        }
        
        // Обновляет поля задачи
        result.Title = request.Title;
        result.Description = request.Description;
        result.Status = request.Status;
        result.Priority = request.Priority;
        result.ModifiedDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        result =  await _taskRepository.Update(result);
        
        // Возвращает обновлённые данные задачи через маппинг
        return _mapper.Map<TaskDescriptionResponse>(result);
    }

    public async Task<TaskResponse> Delete(Guid id)
    {
        // Проверка существования пользователя по ID, если такого нет - выбрасывает исключение
        var result = await _taskRepository.GetById(id);

        if (result is null)
        {
            throw new TaskException("Задача с таким ID не найдена. Повторите попытку");
        }
        
        // Удаляет задачу
        result = await _taskRepository.Delete(result);
        
        // Возвращает информацию об удалённой задачи через маппинг
        return _mapper.Map<TaskResponse>(result);
    }
}