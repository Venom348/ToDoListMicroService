using FluentValidation;
using ToDoList.Contracts.Requests.Auth;
using ToDoList.Contracts.Requests.User;

namespace Auth.Core.Validations;

/// <summary>
///     Валидатор для запроса регистрации пользователя (Register)
/// </summary>
public class PostRegisterRequestValidator : AbstractValidator<PostRegisterRequest>
{
    public PostRegisterRequestValidator()
    {
        // Правила валидации для имени
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя пользователя обязательно!")
            .MinimumLength(3).WithMessage("Минимум 3 символа!")
            .MaximumLength(20).WithMessage("Максимум 20 символов!");
        
        // Правила валидации для фамилии
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия пользователя обязательно!")
            .MinimumLength(3).WithMessage("Минимум 3 символа!")
            .MaximumLength(20).WithMessage("Максимум 20 символов!");
        
        // Правила валидации для Email
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен!")
            .EmailAddress().WithMessage("Некорректный формат Email!")
            .MinimumLength(5).WithMessage("Минимальная длина Email 5 символов!")
            .MaximumLength(30).WithMessage("Максимальная длина Email 30 символов!");
        
        // Правила валидации для пароля
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен!")
            .MinimumLength(8).WithMessage("Минимум 8 символов")
            .MaximumLength(16).WithMessage("Максимум 16 символов")
            .Must(password => !password.Contains(" ")).WithMessage("Пароль не должен содержать пробелы!")
            .Matches(@"[A-Z]+").WithMessage("Пароль должен содержать хотя бы одну заглавную букву!")
            .Matches(@"[a-z]+").WithMessage("Пароль должен содержать хотя бы одну строчную букву!")
            .Matches(@"[0-9]+").WithMessage("Пароль должен содержать хотя бы одну цифру!");
    }
}