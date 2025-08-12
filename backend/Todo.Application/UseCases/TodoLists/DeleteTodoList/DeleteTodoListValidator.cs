using FluentValidation;

namespace Todo.Application.UseCases.TodoLists.DeleteTodoList;

public sealed class DeleteTodoListValidator : AbstractValidator<DeleteTodoListCommand>
{
    public DeleteTodoListValidator()
    {
        // CurrentUserId validator
        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("CurrentUserId is required. Maybe X-User-Id header is missing or invalid");

        // Id validator
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}