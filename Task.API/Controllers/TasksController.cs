using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Task.Core.Abstractions.Services;
using Task.Core.Exceptions;
using ToDoList.Contracts.Requests.Task;

namespace Task.API.Controllers;

/// <summary>
///     Контроллер задачи
/// </summary>
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IValidator<PostTaskRequest> _taskValidator;

    public TasksController(ITaskService taskService, IValidator<PostTaskRequest> taskValidator)
    {
        _taskService = taskService;
        _taskValidator = taskValidator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? title, int page = 0, int limit = 20)
    {
        try
        {
            var response = await _taskService.Get(title, page, limit);
            return Ok(response);
        }
        catch (TaskException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Неизвестная ошибка");
        }
        
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PostTaskRequest request)
    {
        var validationResult = await _taskValidator.ValidateAsync(request);

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
            var response = await _taskService.Create(request);
            return Ok(response);
        }
        catch (TaskException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Неизвестная ошибка");
        }
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] PatchTaskRequest request)
    {
        try
        {
            var response = await _taskService.Update(request);
            return Ok(response);
        }
        catch (TaskException ex)
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
            var response = await _taskService.Delete(id);
            return Ok(response);
        }
        catch (TaskException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Неизвестная ошибка");
        }
    }
}