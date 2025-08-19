namespace Todo.Application.UseCases.Users.UpdateUser;

using Common;
using FluentResults;
using Interfaces;

public class UpdateUserUseCase(IUserRepository userRepository) : IUpdateUserUseCase
{
    /// <inheritdoc />
    public async Task<Result> ExecuteAsync(UpdateUserCommand command, CancellationToken ct = default)
    {
        var existing = await userRepository.GetByIdAsync(command.Id, ct);
        if (existing == null)
            return Result.Fail(Errors.NotFound("User", command.Id));

        // check for email conflict if changed
        if (!string.Equals(existing.Email, command.Email, StringComparison.OrdinalIgnoreCase))
        {
            var conflict = await userRepository.GetByEmailAsync(command.Email, ct);
            if (conflict != null)
                return Result.Fail(Errors.Conflict("Email is already in use"));
        }

        existing.Email = command.Email;
        existing.FullName = command.FullName;

        await userRepository.UpdateAsync(existing, ct);

        return Result.Ok();
    }
}