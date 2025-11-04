using AutoMapper;
using ToDoList.Contracts.Entities;
using ToDoList.Contracts.Responses.User;

namespace Auth.Core.Mapping;

/// <summary>
///     Профиль автомаппера для аутентификации
/// </summary>
public class AuthProfile : Profile
{
    // Создания маппинга для сущности
    public AuthProfile()
    {
        CreateMap<User, UserDescriptionResponse>();
        CreateMap<UserDescriptionResponse, User>();
    }
}