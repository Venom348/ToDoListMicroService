using FluentValidation;
using ToDoList.Contracts.Requests.Task;

namespace Task.Core.Validations;

/// <summary>
///     Валидатор для запроса создания новой задачи 
/// </summary>
public class PostTaskRequestValidator : AbstractValidator<PostTaskRequest>
{
    public PostTaskRequestValidator()
    {
        // Правила валидации для названия задачи
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название задачи обязательно!")
            .MinimumLength(5).WithMessage("Минимум 5 символов!")
            .MaximumLength(256).WithMessage("Максимум 256 символов!");
        
        // Правила валидации для описания задачи
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание задачи обязательно!")
            .MinimumLength(10).WithMessage("Минимум 10 символов!")
            .MaximumLength(256).WithMessage("Максимум 256 символов!");
    }
}