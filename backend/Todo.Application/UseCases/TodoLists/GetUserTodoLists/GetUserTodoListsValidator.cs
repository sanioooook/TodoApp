using FluentValidation;

namespace Todo.Application.UseCases.TodoLists.GetUserTodoLists;

public sealed class GetUserTodoListsValidator : AbstractValidator<GetUserTodoListsQuery>
{
    public GetUserTodoListsValidator()
    {
        // UserId validator
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required. Maybe X-User-Id header is missing or invalid");

        // Skip validator
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0).WithMessage("Skip can't be less of 0");

        // Take validator
        RuleFor(x => x.Take)
            .GreaterThanOrEqualTo(1).WithMessage("Take can't be less of 1");
    }
}