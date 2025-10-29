using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Contracts.Requests.User;
using User.Core.Abstractions.Services;
using User.Core.Exceptions;

namespace User.API.Controllers;

/// <summary>
///     Контроллер пользователя
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<PostUserRequest> _userValidator;

    public UsersController(IUserService userService, IValidator<PostUserRequest> userValidator)
    {
        _userService = userService;
        _userValidator = userValidator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string email)
    {
        try
        {
            //email = HttpContext.User.Claims.FirstOrDefault().Value;
            var response = await _userService.Get(email);
            return Ok(response);
        }
        catch (UserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Неизвестная ошибка");
        }
    }
    
    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] PatchUserRequest request)
    {
        try
        {
            var response = await _userService.Update(request);
            return Ok(response);
        }
        catch (UserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Неизвестная ошибка");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var response = await _userService.Delete(id);
            return Ok(response);
        }
        catch (UserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Неизвестная ошибка");
        }
    }
}