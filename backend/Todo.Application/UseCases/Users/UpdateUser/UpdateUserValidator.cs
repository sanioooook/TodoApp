using FluentValidation;

namespace Todo.Application.UseCases.Users.UpdateUser;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        // Email validator
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        // FullName validator
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required")
            .MaximumLength(255).WithMessage("Maximum length FullName is 255 symbols");

        // Id validator
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}