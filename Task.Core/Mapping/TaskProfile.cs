using AutoMapper;
using ToDoList.Contracts.Responses;
using ToDoList.Contracts.Responses.Task;

namespace Task.Core.Mapping;

/// <summary>
///     Профиль автомаппера для задачи
/// </summary>
public class TaskProfile : Profile
{
    // Создания маппинга для сущности
    public TaskProfile()
    {
        CreateMap<ToDoList.Contracts.Entities.Task, TaskResponse>();
        CreateMap<TaskResponse, ToDoList.Contracts.Entities.Task>();
        CreateMap<ToDoList.Contracts.Entities.Task, TaskDescriptionResponse>();
        CreateMap<TaskDescriptionResponse, ToDoList.Contracts.Entities.Task>();
    }
}