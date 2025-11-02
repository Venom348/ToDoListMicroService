using AutoMapper;
using ToDoList.Contracts.Entities;
using ToDoList.Contracts.Responses.User;

namespace Auth.Core.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserDescriptionResponse>();
        CreateMap<UserDescriptionResponse, User>();
    }
}