using AutoMapper;
using ToDoList.Contracts.Responses;
using ToDoList.Contracts.Responses.User;

namespace User.Core.Mapping;

/// <summary>
///     Профиль автомаппера для пользователя
/// </summary>
public class UserProfile : Profile
{
    // Создания маппинга для сущности
    public UserProfile()
    {
        CreateMap<ToDoList.Contracts.Entities.User, UserResponse>();
        CreateMap<UserResponse, ToDoList.Contracts.Entities.User>();
        CreateMap<ToDoList.Contracts.Entities.User, UserDescriptionResponse>();
        CreateMap<UserDescriptionResponse, ToDoList.Contracts.Entities.User>();
    }
}