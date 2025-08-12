using FluentValidation;

namespace Todo.Application.UseCases.TodoLists.CreateTodoList;

public sealed class CreateTodoListValidator : AbstractValidator<CreateTodoListCommand>
{
    public CreateTodoListValidator()
    {
        // Title validator
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(1).WithMessage("Minimum length Title is 1 symbol")
            .MaximumLength(255).WithMessage("Maximum length Title is 255 symbol");

        // OwnerId validator
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required");
    }
}