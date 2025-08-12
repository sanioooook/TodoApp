using FluentValidation;

namespace Todo.Application.UseCases.TodoLists.UnshareTodoList;

public sealed class UnshareTodoListValidator : AbstractValidator<UnshareTodoListCommand>
{
    public UnshareTodoListValidator()
    {
        // CurrentUserId validator
        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("CurrentUserId is required. Maybe X-User-Id header is missing or invalid");

        // TargetUserId validator
        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("TargetUserId is required");

        // ListId validator
        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("ListId is required");
    }
}