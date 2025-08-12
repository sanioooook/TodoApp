using FluentValidation;

namespace Todo.Application.UseCases.TodoLists.ShareTodoList;

public sealed class ShareTodoListValidator : AbstractValidator<ShareTodoListCommand>
{
    public ShareTodoListValidator()
    {
        // CurrentUserId validator
        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("CurrentUserId is required. Maybe X-User-Id header is missing or invalid");

        // TargetUserId validator
        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("TargetUserId is required")
            .NotEqual(x => x.CurrentUserId).WithMessage("TargetUserId must not equal CurrentUserId");

        // ListId validator
        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("ListId is required");
    }
}