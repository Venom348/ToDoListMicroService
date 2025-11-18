using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Get(string? email, Guid? id)
    {
        try
        {
            var response = await _userService.Get(email, id);
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
        var validationResult = await _userValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(x => new
            {
                Property = x.PropertyName,
                Errors = x.ErrorMessage
            }));
        }

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
    
    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout(Guid id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Error = "Недействительный токен" });
            }

            await _userService.Logout(userId);
            return Ok(new { Message = "Выход выполнен успешно" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "Неизвестная ошибка" });
        }
    }
}