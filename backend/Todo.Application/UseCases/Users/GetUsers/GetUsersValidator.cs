using FluentValidation;

namespace Todo.Application.UseCases.Users.GetUsers;

public sealed class GetUsersValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersValidator()
    {
        // Skip validator
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0).WithMessage("Skip can't be less of 0");

        // Take validator
        RuleFor(x => x.Take)
            .GreaterThanOrEqualTo(1).WithMessage("Take can't be less of 1");
    }
}