using FluentValidation;

namespace Todo.Application.UseCases.TodoLists.GetTodoList;

public sealed class GetTodoListValidator : AbstractValidator<GetTodoListQuery>
{
    public GetTodoListValidator()
    {
        // UserId validator
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required. Maybe X-User-Id header is missing or invalid");

        // ListId validator
        RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("ListId is required");
    }
}