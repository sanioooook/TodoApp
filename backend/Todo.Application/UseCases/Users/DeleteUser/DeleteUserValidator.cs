namespace Todo.Application.UseCases.Users.DeleteUser;

using FluentValidation;

public sealed class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        // Id validation
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}