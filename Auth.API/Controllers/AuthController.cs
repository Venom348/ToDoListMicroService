using Auth.Core.Abstractions.Services;
using Auth.Core.Exceptions;
using Auth.Core.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Contracts.Requests.Auth;
using ToDoList.Contracts.Requests.User;

namespace Auth.API.Controllers;

/// <summary>
///     Контроллер аутентификации
/// </summary>
[ApiController]
[Route("api/authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<PostRegisterRequest> _registerValidator;
    private readonly IValidator<PostLoginRequest> _loginValidator;
    
    public AuthController(IAuthService authService, IValidator<PostRegisterRequest> registerValidator, IValidator<PostLoginRequest> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] PostRegisterRequest request)
    {
        var validationResult = await _registerValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(x => new
            {
                Proprty = x.PropertyName,
                Errors = x.ErrorMessage
            }));
        }

        try
        {
            await _authService.Register(request);
            return Ok();
        }
        catch (AuthException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Неизвестная ошибка");
        }
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] PostLoginRequest request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(x => new
            {
                Proprty = x.PropertyName,
                Errors = x.ErrorMessage
            }));
        }
        
        try
        {
            await _authService.Login(request);
            return Ok();
        }
        catch (AuthException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Неизвестная ошибка");
        }
    }
}