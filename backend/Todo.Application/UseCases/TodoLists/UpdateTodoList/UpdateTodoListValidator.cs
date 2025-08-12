using FluentValidation;

namespace Todo.Application.UseCases.TodoLists.UpdateTodoList;

public sealed class UpdateTodoListValidator : AbstractValidator<UpdateTodoListCommand>
{
    public UpdateTodoListValidator()
    {
        // CurrentUserId validator
        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("CurrentUserId is required. Maybe X-User-Id header is missing or invalid");

        // Title validator
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(1).WithMessage("Minimum length Title is 1 symbol")
            .MaximumLength(255).WithMessage("Maximum length Title is 255 symbol");

        // Id validator
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}