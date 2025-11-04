using System.Security.Claims;
using Auth.Core.Abstractions.Services;
using Auth.Core.Exceptions;
using Auth.Core.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
                Property = x.PropertyName,
                Errors = x.ErrorMessage
            }));
        }

        try
        {
            await _authService.Register(request);
            return Ok(new { Message = "Регистрация прошла успешно!" });
        }
        catch (AuthException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "Неизвестная ошибка" });
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
            var response = await _authService.Login(request);
            return Ok(response);
        }
        catch (AuthException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "Неизвестная ошибка" });
        }
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Error = "Недействительный токен" });
            }
            
            var response = await _authService.RefreshToken(request, userId);
            return Ok(response);
        }
        catch (AuthException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "Неизвестная ошибка" });
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

            await _authService.Logout(userId);
            return Ok(new { Message = "Выход выполнен успешно" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "Неизвестная ошибка" });
        }
    }
}