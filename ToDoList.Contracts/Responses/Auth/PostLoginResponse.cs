using ToDoList.Contracts.Responses.User;

namespace ToDoList.Contracts.Responses.Auth;

public class PostLoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserDescriptionResponse User { get; set; }
}