using FluentValidation;

namespace Todo.Application.UseCases.Users.GetUserById;

public sealed class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        // Id validator
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}